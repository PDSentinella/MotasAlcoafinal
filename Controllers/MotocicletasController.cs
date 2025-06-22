using Microsoft.AspNetCore.Mvc;
using motasAlcoafinal.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MotasAlcoafinal.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using MotasAlcoafinal.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace MotasAlcoafinal.Controllers
{
    [Authorize]
    public class MotocicletasController : Controller
    {
        private readonly MotasAlcoaContext _context;
        private readonly IHubContext<NotificationHub> _hubContext;

        public MotocicletasController(MotasAlcoaContext context, IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        /// <summary>
        /// Exibe a lista de motocicletas com paginação e pesquisa
        /// </summary>
        /// <param name="searchString">Termo de pesquisa</param>
        /// <param name="pageNumber">Número da página</param>
        /// <param name="pageSize">Tamanho da página</param>
        [Authorize]
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

        /// <summary>
        /// Exibe os detalhes de uma motocicleta específica
        /// </summary>
        /// <param name="id">ID da motocicleta</param>
        [Authorize]
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

        /// <summary>
        /// Exibe o formulário de edição de motocicleta
        /// </summary>
        /// <param name="id">ID da motocicleta</param>
        [Authorize(Roles = "Mecanico,Root")]
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

        /// <summary>
        /// Processa a edição de uma motocicleta
        /// </summary>
        /// <param name="id">ID da motocicleta</param>
        /// <param name="motocicleta">Dados da motocicleta</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Mecanico,Root")]
        public async Task<IActionResult> Edit(int id,[Bind("Id, ClienteId, Marca, Modelo, Ano, Placa")] Motocicletas motocicleta)
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
                    await _hubContext.Clients.All.SendAsync("AtualizarMotocicletas");
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

        /// <summary>
        /// Verifica se uma motocicleta existe
        /// </summary>
        /// <param name="id">ID da motocicleta</param>
        [Authorize]
        private bool MotocicletaExists(int id)
        {
            return _context.Motocicletas.Any(e => e.Id == id);
        }

        /// <summary>
        /// Exibe o formulário de criação da motocicleta
        /// </summary>
        [Authorize(Roles = "Mecanico,Root")]
        public IActionResult Create()
        {
            ViewBag.Clientes = new SelectList(_context.Clientes, "Id", "Nome");
            return View();
        }

        /// <summary>
        /// Processa a criação de uma nova motocicleta
        /// </summary>
        /// <param name="motocicleta">Dados da motocicleta</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Mecanico,Root")]
        public async Task<IActionResult> Create([Bind("Marca, Modelo, Ano, Placa, ClienteId")] Motocicletas motocicleta)
        {
            ViewBag.Clientes = new SelectList(_context.Clientes, "Id", "Nome", motocicleta.ClienteId);

            if (ModelState.IsValid)
            {
                // Normaliza a matrícula para maiúsculas
                motocicleta.Placa = motocicleta.Placa.ToUpperInvariant();
                _context.Add(motocicleta);
                await _context.SaveChangesAsync();
                await _hubContext.Clients.All.SendAsync("AtualizarMotocicletas");
                return RedirectToAction(nameof(Index));
            }

            return View(motocicleta);
        }

        /// <summary>
        /// Exibe o formulário de confirmação para remover uma motocicleta
        /// </summary>
        [Authorize(Roles = "Mecanico,Root")]
        public async Task<IActionResult> Delete(int id)
        {
            var motocicleta = await _context.Motocicletas
                .Include(m => m.Servicos)
                .Include(m => m.Cliente)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (motocicleta == null)
            {
                return NotFound();
            }
            ViewBag.HasDependencies = motocicleta.Servicos.Any();
            return View(motocicleta);
        }

        /// <summary>
        /// Processa a remoção de uma motocicleta
        /// </summary>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Mecanico,Root")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var motocicleta = await _context.Motocicletas
                .Include(m => m.Servicos)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (motocicleta == null)
            {
                return NotFound();
            }
            if (motocicleta.Servicos.Any())
            {
                TempData["Error"] = "Não é possível eliminar uma motocicleta com serviços associados.";
                return RedirectToAction("Details", new { id });
            }
            _context.Motocicletas.Remove(motocicleta);
            await _context.SaveChangesAsync();
            await _hubContext.Clients.All.SendAsync("AtualizarMotocicletas");
            TempData["Success"] = "Motocicleta eliminada com sucesso.";
            return RedirectToAction(nameof(Index));
        }
    }
}