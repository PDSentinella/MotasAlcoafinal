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

        public async Task<IActionResult> Details(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }
            return View(cliente);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }
            return View(cliente);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Clientes cliente)
        {
            if (id != cliente.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cliente);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClienteExists(cliente.Id))
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
            return View(cliente);
        }

        private bool ClienteExists(int id)
        {
            return _context.Clientes.Any(e => e.Id == id);
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
