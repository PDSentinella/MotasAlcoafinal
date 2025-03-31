using Microsoft.AspNetCore.Mvc;
using motasAlcoafinal.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MotasAlcoafinal.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace MotasAlcoafinal.Controllers
{
    [Authorize]
    public class MotocicletasController : Controller
    {
        private readonly MotasAlcoaContext _context;

        public MotocicletasController(MotasAlcoaContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchString, int pageNumber = 1, int pageSize = 10)
        {
            var motocicletas = from m in _context.Motocicletas
                               select m;

            if (!string.IsNullOrEmpty(searchString))
            {
                motocicletas = motocicletas.Where(s => s.Marca.Contains(searchString) || s.Modelo.Contains(searchString));
            }

            var totalMotocicletas = await motocicletas.CountAsync();
            var motocicletasList = await motocicletas
                .Include(m => m.Cliente)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.TotalPages = (int)Math.Ceiling(totalMotocicletas / (double)pageSize);
            ViewBag.CurrentPage = pageNumber;
            ViewBag.SearchString = searchString;

            return View(motocicletasList);
        }
        public async Task<IActionResult> Details(int id)
        {
            var motocicleta = await _context.Motocicletas
                .Include(m => m.Cliente)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (motocicleta == null)
            {
                return NotFound();
            }
            return View(motocicleta);
        }
        public async Task<IActionResult> Edit(int id)
        {
            var motocicleta = await _context.Motocicletas.FindAsync(id);
            if (motocicleta == null)
            {
                return NotFound();
            }
            ViewBag.Clientes = new SelectList(_context.Clientes, "Id", "Nome", motocicleta.ClienteId);
            return View(motocicleta);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Motocicletas motocicleta)
        {
            if (id != motocicleta.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(motocicleta);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MotocicletaExists(motocicleta.Id))
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
            ViewBag.Clientes = new SelectList(_context.Clientes, "Id", "Nome", motocicleta.ClienteId);
            return View(motocicleta);
        }
        private bool MotocicletaExists(int id)
        {
            return _context.Motocicletas.Any(e => e.Id == id);
        }
        public IActionResult Create()
        {
            ViewBag.Clientes = new SelectList(_context.Clientes, "Id", "Nome");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Motocicletas motocicleta)
        {
            if (ModelState.IsValid)
            {
                _context.Add(motocicleta);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Clientes = new SelectList(_context.Clientes, "Id", "Nome", motocicleta.ClienteId);
            return View(motocicleta);
        }
    }
}
