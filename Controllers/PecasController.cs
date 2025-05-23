using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using motasAlcoafinal.Models;
using MotasAlcoafinal.Models;

namespace MotasAlcoafinal.Controllers
{
    [Authorize]
    public class PecasController : Controller
    {
        private readonly MotasAlcoaContext _context;

        public PecasController(MotasAlcoaContext context)
        {
            _context = context;
        }

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

        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("Nome, Descricao, Preco, QuantidadeEstoque")] Pecas peca)
        {
            if (ModelState.IsValid)
            {
                _context.Add(peca);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(peca);
        }

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

        [Authorize]
        private bool PecaExists(int id)
        {
            return _context.Pecas.Any(e => e.Id == id);
        }
    }
}
