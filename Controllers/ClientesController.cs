using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using motasAlcoafinal.Models;
using MotasAlcoafinal.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using MotasAlcoafinal.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace MotasAlcoafinal.Controllers
{
    [Authorize]
    public class ClientesController : Controller
    {
        private readonly MotasAlcoaContext _context;
        private readonly IHubContext<NotificationHub> _hubContext;

        public ClientesController(MotasAlcoaContext context, IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        /// <summary>
        /// Exibe a lista de clientes com paginação e pesquisa
        /// </summary>
        /// <param name="searchString">Termo de pesquisa</param>
        /// <param name="pageNumber">Número da página</param>
        /// <param name="pageSize">Tamanho da página</param>
        [Authorize]
        public async Task<IActionResult> Index(string searchString, int pageNumber = 1, int pageSize = 10)
        {
            var clientes = from c in _context.Clientes
                           select c;

            if (!string.IsNullOrEmpty(searchString))
            {
                clientes = clientes.Where(s => s.Nome.Contains(searchString) || s.Email.Contains(searchString));
            }

            var totalClientes = await clientes.CountAsync();
            var clientesList = await clientes
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.TotalPages = (int)Math.Ceiling(totalClientes / (double)pageSize);
            ViewBag.CurrentPage = pageNumber;
            ViewBag.SearchString = searchString;

            return View(clientesList);
        }

        /// <summary>
        /// Exibe os detalhes de um cliente específico
        /// </summary>
        /// <param name="id">ID do cliente</param>
        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            var cliente = await _context.Clientes
                .Include(c => c.Motocicletas)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (cliente == null)
            {
                return NotFound();
            }
            return View(cliente);
        }

        /// <summary>
        /// Exibe o formulário de edição de cliente
        /// </summary>
        /// <param name="id">ID do cliente</param>
        [Authorize(Roles = "Mecanico,Root")]
        public async Task<IActionResult> Edit(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }
            return View(cliente);
        }

        /// <summary>
        /// Processa a edição de um cliente
        /// </summary>
        /// <param name="id">ID do cliente</param>
        /// <param name="cliente">Dados do cliente</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Mecanico,Root")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Telefone,Email,Endereco")] Clientes cliente)
        {
            if (id != cliente.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cliente);
                    await _context.SaveChangesAsync();
                    await _hubContext.Clients.All.SendAsync("AtualizarClientes");
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

        /// <summary>
        /// Verifica se um cliente existe
        /// </summary>
        /// <param name="id">ID do cliente</param>
        [Authorize]
        private bool ClienteExists(int id)
        {
            return _context.Clientes.Any(e => e.Id == id);
        }

        /// <summary>
        /// Exibe o formulário de criação de cliente
        /// </summary>
        [Authorize(Roles = "Mecanico,Root")]
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Processa a criação de um novo cliente
        /// </summary>
        /// <param name="cliente">Dados do cliente</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Mecanico,Root")]
        public async Task<IActionResult> Create([Bind("Nome,Telefone,Email,Endereco")] Clientes cliente)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cliente);
                await _context.SaveChangesAsync();
                await _hubContext.Clients.All.SendAsync("AtualizarClientes");
                return RedirectToAction(nameof(Index));
            }
            return View(cliente);
        }

        /// <summary>
        /// Remove um cliente
        /// </summary>
        /// <param name="id">ID do cliente</param>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Mecanico,Root")]
        public async Task<IActionResult> Delete(int id)
        {
            var cliente = await _context.Clientes
                .Include(c => c.Motocicletas)
                //.Include(c => c.Servicos)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (cliente == null)
                return NotFound();

            bool hasDependencies = (cliente.Motocicletas.Any() || cliente.Servicos.Any());
            ViewBag.HasDependencies = hasDependencies;
            return View(cliente);
        }
    }
}