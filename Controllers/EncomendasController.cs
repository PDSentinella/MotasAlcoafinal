using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using motasAlcoafinal.Models;
using MotasAlcoafinal.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using MotasAlcoafinal.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace MotasAlcoafinal.Controllers
{
    public class EncomendasController : Controller
    {
        private readonly MotasAlcoaContext _context;
        private readonly IHubContext<NotificationHub> _hubContext;

        public EncomendasController(MotasAlcoaContext context, IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        /// <summary>
        /// Exibe a lista de encomendas com paginação, pesquisa e filtro por status
        /// </summary>
        /// <param name="searchString">Termo de pesquisa</param>
        /// <param name="statusFilter">Filtro por status</param>
        /// <param name="pageNumber">Número da página</param>
        /// <param name="pageSize">Tamanho da página</param>
        [Authorize]
        public async Task<IActionResult> Index(string searchString, string statusFilter, int pageNumber = 1, int pageSize = 10)
        {
            var encomendas = from e in _context.Encomendas
                             select e;

            if (!string.IsNullOrEmpty(searchString))
            {
                encomendas = encomendas.Where(e => e.Status.ToString().Contains(searchString));
            }

            if (!string.IsNullOrEmpty(statusFilter))
            {
                encomendas = encomendas.Where(e => e.Status.ToString() == statusFilter);
            }

            var totalEncomendas = await encomendas.CountAsync();
            var encomendasList = await encomendas
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Include(e => e.EncomendaPecas).ThenInclude(ep => ep.Peca)
                .ToListAsync();

            ViewBag.TotalPages = (int)Math.Ceiling(totalEncomendas / (double)pageSize);
            ViewBag.CurrentPage = pageNumber;
            ViewBag.SearchString = searchString;
            ViewBag.StatusFilter = statusFilter;

            return View(encomendasList);
        }

        /// <summary>
        /// Exibe os detalhes de uma encomenda específica
        /// </summary>
        /// <param name="id">ID da encomenda</param>
        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            var encomenda = await _context.Encomendas
                .Include(e => e.EncomendaPecas)
                .ThenInclude(ep => ep.Peca)
                .FirstOrDefaultAsync(e => e.Id == id);
            if (encomenda == null)
            {
                return NotFound();
            }
            return View(encomenda);
        }

        /// <summary>
        /// Exibe o formulário de criação da encomenda
        /// </summary>
        [Authorize(Roles = "Gestor,Root")]
        public IActionResult Create()
        {
            ViewBag.Pecas = new SelectList(_context.Pecas, "Id", "Nome");
            return View();
        }

        /// <summary>
        /// Processa a criação de uma nova encomenda
        /// </summary>
        /// <param name="encomenda">Dados da encomenda</param>
        /// <param name="pecasIds">IDs das peças</param>
        /// <param name="quantidades">Quantidades das peças (array de inteiros)</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Gestor,Root")]
        public async Task<IActionResult> Create([Bind("DataPedido, Status ")] Encomendas encomenda, List<int> pecasIds, List<int> quantidades)
        {
            // Validar se as listas existem e têm o mesmo tamanho
            if (pecasIds == null || quantidades == null || pecasIds.Count != quantidades.Count || pecasIds.Count == 0)
            {
                ModelState.AddModelError("", "Selecione as peças e insira as quantidades correspondentes.");
                ViewBag.Pecas = new SelectList(_context.Pecas, "Id", "Nome");
                return View(encomenda);
            }

            // Validar se todas as quantidades são positivas
            for (int i = 0; i < quantidades.Count; i++)
            {
                if (quantidades[i] <= 0)
                {
                    ModelState.AddModelError("", "Por favor, insira apenas números inteiros positivos nas quantidades.");
                    ViewBag.Pecas = new SelectList(_context.Pecas, "Id", "Nome");
                    return View(encomenda);
                }
            }

            if (ModelState.IsValid)
            {
                encomenda.Status = Encomendas.EncomendaEstado.Pendente; // Sempre Pendente ao criar

                _context.Add(encomenda);
                await _context.SaveChangesAsync();

                for (int i = 0; i < pecasIds.Count; i++)
                {
                    var encomendaPeca = new EncomendaPecas
                    {
                        EncomendaId = encomenda.Id,
                        PecaId = pecasIds[i],
                        Quantidade = quantidades[i]
                    };
                    _context.EncomendaPecas.Add(encomendaPeca);
                }

                await _context.SaveChangesAsync();
                await _hubContext.Clients.All.SendAsync("AtualizarEncomendas");
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Pecas = new SelectList(_context.Pecas, "Id", "Nome");
            return View(encomenda);
        }

        /// <summary>
        /// Exibe o formulário de edição da encomenda
        /// </summary>
        /// <param name="id">ID da encomenda</param>
        [Authorize(Roles = "Gestor,Root")]
        public async Task<IActionResult> Edit(int id)
        {
            var encomenda = await _context.Encomendas
                .Include(e => e.EncomendaPecas)
                .FirstOrDefaultAsync(e => e.Id == id);
            if (encomenda == null)
            {
                return NotFound();
            }
            ViewBag.Pecas = new SelectList(_context.Pecas, "Id", "Nome");
            return View(encomenda);
        }

        /// <summary>
        /// Processa a edição de uma encomenda
        /// </summary>
        /// <param name="id">ID da encomenda</param>
        /// <param name="encomenda">Dados da encomenda</param>
        /// <param name="pecasIds">IDs das peças</param>
        /// <param name="quantidades">Quantidades das peças</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Gestor,Root")]
        public async Task<IActionResult> Edit(int id, Encomendas encomenda, List<int> pecasIds, List<int> quantidades)
        {
            if (id != encomenda.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Buscar o estado anterior da encomenda
                    var encomendaAntiga = await _context.Encomendas
                        .Include(e => e.EncomendaPecas)
                        .FirstOrDefaultAsync(e => e.Id == id);

                    var statusAnterior = encomendaAntiga.Status;

                    // Impede voltar de Entregue para outro estado
                    if (statusAnterior == Encomendas.EncomendaEstado.Entregue && encomenda.Status != statusAnterior)
                    {
                        TempData["Error"] = "Não é permitido alterar o estado de 'Entregue' para outro estado.";
                        ViewBag.Pecas = new SelectList(_context.Pecas, "Id", "Nome");
                        return View(encomendaAntiga);
                    }
                    // Impede voltar de Cancelada para outro estado
                    if (statusAnterior == Encomendas.EncomendaEstado.Cancelada && encomenda.Status != statusAnterior)
                    {
                        TempData["Error"] = "Não é permitido alterar o estado de 'Cancelada' para outro estado.";
                        ViewBag.Pecas = new SelectList(_context.Pecas, "Id", "Nome");
                        return View(encomendaAntiga);
                    }

                    // Impede alteração da data se Entregue ou Cancelada
                    if (statusAnterior == Encomendas.EncomendaEstado.Entregue || statusAnterior == Encomendas.EncomendaEstado.Cancelada)
                    {
                        encomenda.DataPedido = encomendaAntiga.DataPedido;
                    }

                    _context.Entry(encomendaAntiga).CurrentValues.SetValues(encomenda);
                    await _context.SaveChangesAsync();
                    await _hubContext.Clients.All.SendAsync("AtualizarEncomendas"); 

                    // Se mudou de Pendente para Entregue, atualizar estoque
                    if (statusAnterior == Encomendas.EncomendaEstado.Pendente && encomenda.Status == Encomendas.EncomendaEstado.Entregue)
                    {
                        // Buscar as peças da encomenda antiga
                        var encomendaPecas = await _context.EncomendaPecas.Where(ep => ep.EncomendaId == id).ToListAsync();
                        foreach (var ep in encomendaPecas)
                        {
                            var peca = await _context.Pecas.FirstOrDefaultAsync(p => p.Id == ep.PecaId);
                            if (peca != null)
                            {
                                peca.QuantidadeEstoque += ep.Quantidade;
                            }
                        }
                        await _context.SaveChangesAsync();
                    }

                    // Atualizar as peças da encomenda
                    if (pecasIds != null && quantidades != null && pecasIds.Count == quantidades.Count && pecasIds.Count > 0)
                    {
                        var existingEncomendaPecas = _context.EncomendaPecas.Where(ep => ep.EncomendaId == id).ToList();
                        _context.EncomendaPecas.RemoveRange(existingEncomendaPecas);
                        await _context.SaveChangesAsync();
                        for (int i = 0; i < pecasIds.Count; i++)
                        {
                            var encomendaPeca = new EncomendaPecas
                            {
                                EncomendaId = encomenda.Id,
                                PecaId = pecasIds[i],
                                Quantidade = quantidades[i]
                            };
                            _context.EncomendaPecas.Add(encomendaPeca);
                        }
                        await _context.SaveChangesAsync();
                    }
                    // Se não vierem peças novas, não remove as antigas!
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EncomendaExists(encomenda.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Pecas = new SelectList(_context.Pecas, "Id", "Nome");
            return View(encomenda);
        }

        /// <summary>
        /// Exibe a página de confirmação para eliminar uma encomenda
        /// </summary>
        /// <param name="id">ID da encomenda</param>
        [Authorize(Roles = "Gestor,Root")]
        public async Task<IActionResult> Delete(int id)
        {
            var encomenda = await _context.Encomendas
                .Include(e => e.EncomendaPecas)
                .ThenInclude(ep => ep.Peca)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (encomenda == null)
            {
                return NotFound();
            }

            return View(encomenda);
        }

        /// <summary>
        /// Confirma a eliminação de uma encomenda
        /// </summary>
        /// <param name="id">ID da encomenda</param>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Gestor,Root")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var encomenda = await _context.Encomendas
                .Include(e => e.EncomendaPecas)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (encomenda == null)
            {
                return NotFound();
            }

            if (encomenda.Status == Encomendas.EncomendaEstado.Entregue ||
                encomenda.Status == Encomendas.EncomendaEstado.Cancelada)
            {
                TempData["Error"] = "Não é possível eliminar uma encomenda que já foi entregue ou cancelada.";
                return RedirectToAction("Details", new { id });
            }

            // Remove as peças da encomenda primeiro (devido à relação)
            if (encomenda.EncomendaPecas.Any())
            {
                _context.EncomendaPecas.RemoveRange(encomenda.EncomendaPecas);
            }

            // Remove a encomenda
            _context.Encomendas.Remove(encomenda);
            await _context.SaveChangesAsync();

            await _hubContext.Clients.All.SendAsync("AtualizarEncomendas");

            TempData["Success"] = "Encomenda eliminada com sucesso.";
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Verifica se uma encomenda existe
        /// </summary>
        /// <param name="id">ID da encomenda</param>
        private bool EncomendaExists(int id)
        {
            return _context.Encomendas.Any(e => e.Id == id);
        }
    }
}