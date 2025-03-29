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
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Servicos servico)
        {
            if (ModelState.IsValid)
            {
                _context.Add(servico);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Motocicletas = new SelectList(_context.Motocicletas, "Id", "Modelo", servico.MotocicletaId);
            return View(servico);
        }
        public async Task<IActionResult> Edit(int id)
        {
            var servico = await _context.Servicos.FindAsync(id);
            if (servico == null)
            {
                return NotFound();
            }
            ViewBag.Motocicletas = new SelectList(_context.Motocicletas, "Id", "Modelo", servico.MotocicletaId);
            return View(servico);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Servicos servico)
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
            return View(servico);
        }

        private bool ServicoExists(int id)
        {
            return _context.Servicos.Any(e => e.Id == id);
        }

    }
}
