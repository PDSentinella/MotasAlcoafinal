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
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.TotalPages = (int)Math.Ceiling(totalServicos / (double)pageSize);
            ViewBag.CurrentPage = pageNumber;
            ViewBag.SearchString = searchString;

            return View(servicosList);
        }
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

        public IActionResult Create()
        {
            ViewBag.Motocicletas = new SelectList(_context.Motocicletas, "Id", "Modelo");
            ViewBag.Pecas = new SelectList(_context.Pecas, "Id", "Nome");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Servicos servico, List<int> pecasIds, List<int> quantidades)
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
                }
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            ViewBag.Motocicletas = new SelectList(_context.Motocicletas, "Id", "Modelo", servico.MotocicletaId);
            ViewBag.Pecas = new SelectList(_context.Pecas, "Id", "Nome");
            return View(servico);
        }
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
        public async Task<IActionResult> Edit(int id, Servicos servico, List<int> pecasIds, List<int> quantidades)
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
        public IActionResult AddPeca(int id)
        {
            ViewBag.Pecas = new SelectList(_context.Pecas, "Id", "Nome");
            return View(new ServicoPecas { ServicoId = id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPeca(ServicoPecas servicoPeca)
        {
            if (ModelState.IsValid)
            {
                _context.ServicoPecas.Add(servicoPeca);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", new { id = servicoPeca.ServicoId });
            }
            ViewBag.Pecas = new SelectList(_context.Pecas, "Id", "Nome", servicoPeca.PecaId);
            return View(servicoPeca);
        }
        private bool ServicoExists(int id)
        {
            return _context.Servicos.Any(e => e.Id == id);
        }
    }
}
