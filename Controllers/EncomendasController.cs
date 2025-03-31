using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using motasAlcoafinal.Models;
using MotasAlcoafinal.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MotasAlcoafinal.Controllers
{
    public class EncomendasController : Controller
    {
        private readonly MotasAlcoaContext _context;

        public EncomendasController(MotasAlcoaContext context)
        {
            _context = context;
        }

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
                .ToListAsync();

            ViewBag.TotalPages = (int)Math.Ceiling(totalEncomendas / (double)pageSize);
            ViewBag.CurrentPage = pageNumber;
            ViewBag.SearchString = searchString;
            ViewBag.StatusFilter = statusFilter;

            return View(encomendasList);
        }

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

        public IActionResult Create()
        {
            ViewBag.Pecas = new SelectList(_context.Pecas, "Id", "Nome");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Encomendas encomenda, List<int> pecasIds, List<int> quantidades)
        {
            if (ModelState.IsValid)
            {
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

                return RedirectToAction(nameof(Index));
            }
            ViewBag.Pecas = new SelectList(_context.Pecas, "Id", "Nome");
            return View(encomenda);
        }

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

        [HttpPost]
        [ValidateAntiForgeryToken]
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
                    _context.Update(encomenda);
                    await _context.SaveChangesAsync();

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

        private bool EncomendaExists(int id)
        {
            return _context.Encomendas.Any(e => e.Id == id);
        }
    }
}
