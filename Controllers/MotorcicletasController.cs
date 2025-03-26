using Microsoft.AspNetCore.Mvc;
using motasAlcoafinal.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MotasAlcoafinal.Models;

namespace MotasAlcoafinal.Controllers
{
    public class MotocicletasController : Controller
    {
        private readonly MotasAlcoaContext _context;

        public MotocicletasController(MotasAlcoaContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10)
        {
            var motocicletas = await _context.Motocicletas
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var totalMotocicletas = await _context.Motocicletas.CountAsync();
            ViewBag.TotalPages = (int)Math.Ceiling(totalMotocicletas / (double)pageSize);
            ViewBag.CurrentPage = pageNumber;

            return View(motocicletas);
        }

        public IActionResult Create()
        {
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
            return View(motocicleta);
        }
    }
}
