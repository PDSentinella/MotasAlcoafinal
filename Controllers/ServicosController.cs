using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using motasAlcoafinal.Models;
using MotasAlcoafinal.Models;

namespace MotasAlcoafinal.Controllers
{
    public class ServicosController : Controller
    {
        private readonly MotasAlcoaContext _context;

        public ServicosController(MotasAlcoaContext context)
        {
            _context = context;
        }

        [Authorize]
        public async Task<IActionResult> Index(string searchString, int pageNumber = 1, int pageSize = 10)
        {
            var servicos = from s in _context.Servicos
                           select s;

            if (!string.IsNullOrEmpty(searchString))
            {
                servicos = servicos.Where(s => s.Descricao.Contains(searchString));
            }

            var totalServicos = await servicos.CountAsync();
            var servicosList = await servicos
                .Include(s => s.Motocicleta)
                .Include(s => s.ServicoPecas)
                    .ThenInclude(sp => sp.Peca)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.TotalPages = (int)Math.Ceiling(totalServicos / (double)pageSize);
            ViewBag.CurrentPage = pageNumber;
            ViewBag.SearchString = searchString;

            return View(servicosList);
        }

        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            var servico = await _context.Servicos
                .Include(s => s.Motocicleta)
                .Include(s => s.ServicoPecas)
                .ThenInclude(sp => sp.Peca)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (servico == null)
            {
                return NotFound();
            }
            return View(servico);
        }

        [Authorize(Roles = "Mecanico,Root")]
        public IActionResult Create()
        {
            ViewBag.Motocicletas = new SelectList(_context.Motocicletas, "Id", "Modelo");
            ViewBag.Pecas = new SelectList(_context.Pecas, "Id", "Nome");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Mecanico,Root")]
        public async Task<IActionResult> Create( [Bind("Descricao, Data, CustoTotal, MotocicletaId")]Servicos servico, List<int> pecasIds, List<int> quantidades)
        {
            if (ModelState.IsValid)
            {
                _context.Add(servico);
                await _context.SaveChangesAsync();

                for (int i = 0; i < pecasIds.Count; i++)
                {
                    var servicoPeca = new ServicoPecas
                    {
                        ServicoId = servico.Id,
                        PecaId = pecasIds[i],
                        QuantidadeUsada = quantidades[i]
                    };
                    _context.ServicoPecas.Add(servicoPeca);

                    // Descontar do estoque
                    var peca = await _context.Pecas.FindAsync(pecasIds[i]);
                    if (peca != null)
                    {
                        peca.QuantidadeEstoque -= quantidades[i];
                        if (peca.QuantidadeEstoque < 0) peca.QuantidadeEstoque = 0;
                        _context.Pecas.Update(peca);
                    }
                }
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            ViewBag.Motocicletas = new SelectList(_context.Motocicletas, "Id", "Modelo", servico.MotocicletaId);
            ViewBag.Pecas = new SelectList(_context.Pecas, "Id", "Nome");
            return View(servico);
        }
        [Authorize(Roles = "Mecanico,Root")]
        public async Task<IActionResult> Edit(int id)
        {
            var servico = await _context.Servicos
                .Include(s => s.ServicoPecas)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (servico == null)
            {
                return NotFound();
            }
            ViewBag.Motocicletas = new SelectList(_context.Motocicletas, "Id", "Modelo", servico.MotocicletaId);
            ViewBag.Pecas = new SelectList(_context.Pecas, "Id", "Nome");
            return View(servico);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Mecanico,Root")]
        public async Task<IActionResult> Edit(int id, [Bind("Descricao, Data, CustoTotal, MotocicletaId")] Servicos servico, List<int> pecasIds, List<int> quantidades)
        {
            if (id != servico.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(servico);
                    await _context.SaveChangesAsync();

                    var existingServicoPecas = _context.ServicoPecas.Where(sp => sp.ServicoId == id).ToList();
                    // Repor estoque das peças removidas
                    foreach (var sp in existingServicoPecas)
                    {
                        var peca = await _context.Pecas.FindAsync(sp.PecaId);
                        if (peca != null)
                        {
                            peca.QuantidadeEstoque += sp.QuantidadeUsada;
                            _context.Pecas.Update(peca);
                        }
                    }
                    _context.ServicoPecas.RemoveRange(existingServicoPecas);
                    await _context.SaveChangesAsync();

                    for (int i = 0; i < pecasIds.Count; i++)
                    {
                        var servicoPeca = new ServicoPecas
                        {
                            ServicoId = servico.Id,
                            PecaId = pecasIds[i],
                            QuantidadeUsada = quantidades[i]
                        };
                        _context.ServicoPecas.Add(servicoPeca);

                        // Descontar do estoque
                        var peca = await _context.Pecas.FindAsync(pecasIds[i]);
                        if (peca != null)
                        {
                            peca.QuantidadeEstoque -= quantidades[i];
                            if (peca.QuantidadeEstoque < 0) peca.QuantidadeEstoque = 0;
                            _context.Pecas.Update(peca);
                        }
                    }
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServicoExists(servico.Id))
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
            ViewBag.Motocicletas = new SelectList(_context.Motocicletas, "Id", "Modelo", servico.MotocicletaId);
            ViewBag.Pecas = new SelectList(_context.Pecas, "Id", "Nome");
            return View(servico);
        }

        [Authorize(Roles = "Mecanico,Root")]
        public IActionResult AddPeca(int id)
        {
            ViewBag.Pecas = new SelectList(_context.Pecas, "Id", "Nome");
            return View(new ServicoPecas { ServicoId = id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Mecanico,Root")]
        public async Task<IActionResult> AddPeca(ServicoPecas servicoPeca)
        {
            if (ModelState.IsValid && servicoPeca.ServicoId != null)
            {
                // Atualiza o stock da peça
                var peca = await _context.Pecas.FindAsync(servicoPeca.PecaId);
                if (peca != null)
                {
                    peca.QuantidadeEstoque -= servicoPeca.QuantidadeUsada;
                    if (peca.QuantidadeEstoque < 0)
                        peca.QuantidadeEstoque = 0; // Nunca deixa negativo
                    _context.Pecas.Update(peca);
                }

                var ser = new ServicoPecas
                {
                    ServicoId = servicoPeca.ServicoId,
                    PecaId = servicoPeca.PecaId,
                    QuantidadeUsada = servicoPeca.QuantidadeUsada
                };
                _context.ServicoPecas.Add(ser);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", new { id = servicoPeca.ServicoId });
            }
            ViewBag.Pecas = new SelectList(_context.Pecas, "Id", "Nome", servicoPeca.PecaId);
            return View(servicoPeca);
        }
        public async Task<IActionResult> EditPeca(int id)
        {
            var servicoPeca = await _context.ServicoPecas
                .Include(sp => sp.Peca)
                .FirstOrDefaultAsync(sp => sp.Id == id);
            if (servicoPeca == null)
            {
                return NotFound();
            }
            ViewBag.Pecas = new SelectList(_context.Pecas, "Id", "Nome", servicoPeca.PecaId);
            return View(servicoPeca);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Mecanico,Root")]
        public async Task<IActionResult> EditPeca(ServicoPecas servicoPeca)
        {
            if (ModelState.IsValid && servicoPeca.PecaId != null)
            {
                // Buscar o registro antigo
                var antigo = await _context.ServicoPecas.AsNoTracking().FirstOrDefaultAsync(sp => sp.Id == servicoPeca.Id);
                if (antigo != null)
                {
                    var peca = await _context.Pecas.FindAsync(servicoPeca.PecaId);
                    if (peca != null)
                    {
                        // Repor o stock antigo
                        peca.QuantidadeEstoque += antigo.QuantidadeUsada;
                        // Subtrair o novo valor
                        peca.QuantidadeEstoque -= servicoPeca.QuantidadeUsada;
                        if (peca.QuantidadeEstoque < 0)
                            peca.QuantidadeEstoque = 0;
                        _context.Pecas.Update(peca);
                    }
                }

                _context.Update(servicoPeca);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", new { id = servicoPeca.ServicoId });
            }
            ViewBag.Pecas = new SelectList(_context.Pecas, "Id", "Nome", servicoPeca.PecaId);
            return View(servicoPeca);
        }

        [Authorize(Roles = "Mecanico,Root")]
        public async Task<IActionResult> DeletePeca(int id)
        {
            var servicoPeca = await _context.ServicoPecas.FindAsync(id);
            if (servicoPeca == null)
            {
                return NotFound();
            }

            // Repor o stock da peça
            var peca = await _context.Pecas.FindAsync(servicoPeca.PecaId);
            if (peca != null)
            {
                peca.QuantidadeEstoque += servicoPeca.QuantidadeUsada;
                _context.Pecas.Update(peca);
            }

            _context.ServicoPecas.Remove(servicoPeca);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { id = servicoPeca.ServicoId });
        }
        public async Task<IActionResult> EncomendarTodos(int id)
        {
            var servico = await _context.Servicos
                .Include(s => s.ServicoPecas)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (servico == null)
            {
                return NotFound();
            }

            var encomenda = new Encomendas
            {
                DataPedido = DateTime.Now,
                Status = Encomendas.Estados.Pendente
            };

            _context.Encomendas.Add(encomenda);
            await _context.SaveChangesAsync();

            foreach (var servicoPeca in servico.ServicoPecas)
            {
                var encomendaPeca = new EncomendaPecas
                {
                    EncomendaId = encomenda.Id,
                    PecaId = servicoPeca.PecaId,
                    Quantidade = servicoPeca.QuantidadeUsada
                };
                _context.EncomendaPecas.Add(encomendaPeca);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Encomendas", new { id = encomenda.Id });
        }
        private bool ServicoExists(int id)
        {
            return _context.Servicos.Any(e => e.Id == id);
        }
    }
}