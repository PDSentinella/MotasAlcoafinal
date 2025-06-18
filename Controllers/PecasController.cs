using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using motasAlcoafinal.Models;
using MotasAlcoafinal.Models;
using MotasAlcoafinal.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace MotasAlcoafinal.Controllers
{
    [Authorize]
    public class PecasController : Controller
    {
        private readonly MotasAlcoaContext _context;
        private readonly IHubContext<NotificationHub> _hubContext;

        public PecasController(MotasAlcoaContext context,IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        /// <summary>
        /// Exibe a lista de peças com paginação e pesquisa
        /// </summary>
        /// <param name="searchString">Termo de pesquisa</param>
        /// <param name="pageNumber">Número da página</param>
        /// <param name="pageSize">Tamanho da página</param>
        [Authorize]
        public async Task<IActionResult> Index(string searchString, int pageNumber = 1, int pageSize = 10)
        {
            var pecas = from p in _context.Pecas
                        select p;

            if (!string.IsNullOrEmpty(searchString))
            {
                pecas = pecas.Where(s => s.Nome.Contains(searchString) || s.Descricao.Contains(searchString));
            }

            var totalPecas = await pecas.CountAsync();
            var pecasList = await pecas
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.TotalPages = (int)Math.Ceiling(totalPecas / (double)pageSize);
            ViewBag.CurrentPage = pageNumber;
            ViewBag.SearchString = searchString;

            return View(pecasList);
        }

        /// <summary>
        /// Exibe os detalhes de uma peça específica
        /// </summary>
        /// <param name="id">ID da peça</param>
        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            var peca = await _context.Pecas.FindAsync(id);
            if (peca == null)
            {
                return NotFound();
            }
            return View(peca);
        }

        /// <summary>
        /// Exibe o formulário de criação da peça
        /// </summary>
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Processa a criação de uma nova peça
        /// </summary>
        /// <param name="peca">Dados da peça</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("Nome, Descricao, Preco, QuantidadeEstoque")] Pecas peca)
        {
            if (ModelState.IsValid)
            {
                _context.Add(peca);
                await _context.SaveChangesAsync();
                await _hubContext.Clients.All.SendAsync("AtualizarPecas");
                return RedirectToAction(nameof(Index));
            }
            return View(peca);
        }

        /// <summary>
        /// Exibe o formulário de edição da peça
        /// </summary>
        /// <param name="id">ID da peça</param>
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var peca = await _context.Pecas.FindAsync(id);
            if (peca == null)
            {
                return NotFound();
            }
            return View(peca);
        }

        /// <summary>
        /// Processa a edição de uma peça
        /// </summary>
        /// <param name="id">ID da peça</param>
        /// <param name="peca">Dados da peça</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("Id, Nome, Descricao, Preco, QuantidadeEstoque")] Pecas peca)
        {
            if (id != peca.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(peca);
                    await _context.SaveChangesAsync();
                    await _hubContext.Clients.All.SendAsync("AtualizarPecas");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PecaExists(peca.Id))
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
            return View(peca);
        }

        /// <summary>
        /// Exibe o formulário de confirmação para remover uma peça
        /// </summary>
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var peca = await _context.Pecas
                .Include(p => p.ServicoPecas)
                .Include(p => p.EncomendaPecas)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (peca == null)
            {
                return NotFound();
            }
            ViewBag.HasDependencies = (peca.ServicoPecas.Any() || peca.EncomendaPecas.Any());
            return View(peca);
        }

        /// <summary>
        /// Processa a remoção de uma peça
        /// </summary>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var peca = await _context.Pecas
                .Include(p => p.ServicoPecas)
                .Include(p => p.EncomendaPecas)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (peca == null)
            {
                return NotFound();
            }
            if (peca.ServicoPecas.Any() || peca.EncomendaPecas.Any())
            {
                TempData["Error"] = "Não é possível eliminar uma peça com dependências (serviços ou encomendas associadas).";
                return RedirectToAction("Details", new { id });
            }
            _context.Pecas.Remove(peca);
            await _context.SaveChangesAsync();
            await _hubContext.Clients.All.SendAsync("AtualizarPecas");
            TempData["Success"] = "Peça eliminada com sucesso.";
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Verifica se uma peça existe
        /// </summary>
        /// <param name="id">ID da peça</param>
        [Authorize]
        private bool PecaExists(int id)
        {
            return _context.Pecas.Any(e => e.Id == id);
        }
    }
}