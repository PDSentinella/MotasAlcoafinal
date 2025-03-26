using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using motasAlcoafinal.Models;
using MotasAlcoafinal.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace MotasAlcoafinal.Controllers
{
    public class ClientesController : Controller
    {
        private readonly MotasAlcoaContext _context;

        public ClientesController(MotasAlcoaContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10)
        {
            var clientes = await _context.Clientes
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var totalClientes = await _context.Clientes.CountAsync();
            ViewBag.TotalPages = (int)Math.Ceiling(totalClientes / (double)pageSize);
            ViewBag.CurrentPage = pageNumber;

            return View(clientes);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Clientes cliente)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cliente);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cliente);
        }
    }
}
