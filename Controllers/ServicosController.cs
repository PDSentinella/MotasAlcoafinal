using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using motasAlcoafinal.Models;
using MotasAlcoafinal.Models;
using MotasAlcoafinal.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace MotasAlcoafinal.Controllers
{
    public class ServicosController : Controller
    {
        private readonly MotasAlcoaContext _context;
        private readonly IHubContext<NotificationHub> _hubContext;

        public ServicosController(MotasAlcoaContext context, IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        /// <summary>
        /// Exibe a lista de serviços com paginação e pesquisa
        /// </summary>
        /// <param name="searchString">Termo de pesquisa</param>
        /// <param name="pageNumber">Número da página</param>
        /// <param name="pageSize">Tamanho da página</param>
        [Authorize]
        public async Task<IActionResult> Index(string searchString, string statusFilter, int pageNumber = 1, int pageSize = 10)
        {
            var servicos = from s in _context.Servicos
                           select s;

            if (!string.IsNullOrEmpty(searchString))
            {
                servicos = servicos.Where(s => s.Cliente.Nome.Contains(searchString));
            }
            // Filtro pelo estado do serviço
            if (!string.IsNullOrEmpty(statusFilter) &&
                Enum.TryParse<motasAlcoafinal.Models.Servicos.ServicoEstado>(statusFilter, out var status))
            {
                servicos = servicos.Where(s => s.Status == status);
            }

            var totalServicos = await servicos.CountAsync();
            var servicosList = await servicos
                .Include(s => s.Cliente)
                .Include(s => s.Motocicleta)
                .Include(s => s.ServicoPecas)
                    .ThenInclude(sp => sp.Peca)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.TotalPages = (int)Math.Ceiling(totalServicos / (double)pageSize);
            ViewBag.CurrentPage = pageNumber;
            ViewBag.SearchString = searchString;
            ViewBag.StatusFilter = statusFilter;

            return View(servicosList);
        }

        /// <summary>
        /// Exibe os detalhes de um serviço específico
        /// </summary>
        /// <param name="id">ID do serviço</param>
        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            var servico = await _context.Servicos
                .Include(s => s.Cliente)
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

        /// <summary>
        /// Exibe o formulário de criação do serviço
        /// </summary>
        [Authorize(Roles = "Mecanico,Root")]
        public IActionResult Create()
        {
            ViewBag.Clientes = new SelectList(_context.Clientes, "Id", "Nome");
            ViewBag.Pecas = new SelectList(_context.Pecas, "Id", "Nome");
            ViewBag.PecasData = _context.Pecas.ToDictionary(p => p.Id, p => p.Preco);
            ViewBag.PecasObj = _context.Pecas.ToDictionary(p => p.Id, p => p);
            return View();
        }

        /// <summary>
        /// Processa a criação de um novo serviço
        /// </summary>
        /// <param name="servico">Dados do serviço</param>
        /// <param name="pecasIds">IDs das peças</param>
        /// <param name="quantidades">Quantidades das peças</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Mecanico,Root")]
        public async Task<IActionResult> Create([Bind("Descricao, Data, CustoTotal, MotocicletaId")]Servicos servico, int ClienteId, List<int> pecasIds, List<int> quantidades)
        {
            if (ClienteId == 0)
                ModelState.AddModelError("ClienteId", "Selecione um cliente.");
            if (servico.MotocicletaId == null || servico.MotocicletaId == 0)
                ModelState.AddModelError("MotocicletaId", "Selecione uma motocicleta.");
            if (ClienteId > 0 && servico.MotocicletaId > 0)
            {
                var moto = await _context.Motocicletas.FirstOrDefaultAsync(m => m.Id == servico.MotocicletaId && m.ClienteId == ClienteId);
                if (moto == null)
                    ModelState.AddModelError("MotocicletaId", "A motocicleta selecionada não pertence ao cliente escolhido.");
            }
            if (ModelState.IsValid)
            {
                decimal totalPecas = 0;
                for (int i = 0; i < pecasIds.Count; i++)
                {
                    var peca = await _context.Pecas.FindAsync(pecasIds[i]);
                    if (peca != null)
                    {
                        if (quantidades[i] > peca.QuantidadeEstoque)
                        {
                            ModelState.AddModelError("", $"Stock insuficiente para a peça '{peca.Nome}'. Stock disponível: {peca.QuantidadeEstoque}, solicitado: {quantidades[i]}.");
                        }
                        totalPecas += peca.Preco * quantidades[i];
                    }
                }
                if (!ModelState.IsValid)
                {
                    ViewBag.Clientes = new SelectList(_context.Clientes, "Id", "Nome", ClienteId);
                    ViewBag.Pecas = new SelectList(_context.Pecas, "Id", "Nome");
                    ViewBag.PecasData = _context.Pecas.ToDictionary(p => p.Id, p => p.Preco);
                    ViewBag.PecasObj = _context.Pecas.ToDictionary(p => p.Id, p => p);
                    return View(servico);
                }
                servico.CustoTotal += totalPecas;
                servico.Status = Servicos.ServicoEstado.Pendente;
                servico.ClienteId = ClienteId;
                _context.Add(servico);
                await _context.SaveChangesAsync();
                await _hubContext.Clients.All.SendAsync("AtualizarServicos");
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
                await _hubContext.Clients.All.SendAsync("AtualizarServicos");
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Clientes = new SelectList(_context.Clientes, "Id", "Nome", ClienteId);
            ViewBag.Pecas = new SelectList(_context.Pecas, "Id", "Nome");
            ViewBag.PecasData = _context.Pecas.ToDictionary(p => p.Id, p => p.Preco);
            ViewBag.PecasObj = _context.Pecas.ToDictionary(p => p.Id, p => p);
            return View(servico);
        }

        /// <summary>
        /// Exibe o formulário de edição do serviço
        /// </summary>
        /// <param name="id">ID do serviço</param>
        [Authorize(Roles = "Mecanico,Root")]
        public async Task<IActionResult> Edit(int id)
        {
            var servico = await _context.Servicos
                .Include(s => s.Cliente)
                .Include(s => s.Motocicleta)
                .Include(s => s.ServicoPecas)
                .ThenInclude(sp => sp.Peca)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (servico == null)
            {
                return NotFound();
            }
            if (servico.Status == Servicos.ServicoEstado.Concluido || servico.Status == Servicos.ServicoEstado.Cancelado)
            {
                TempData["Error"] = "Não é permitido editar um serviço Concluido ou Cancelado.";
                return RedirectToAction("Details", new { id });
            }
            ViewBag.Clientes = new SelectList(_context.Clientes, "Id", "Nome", servico.ClienteId);
            ViewBag.Motocicletas = new SelectList(_context.Motocicletas.Where(m => m.ClienteId == servico.ClienteId).Select(m => new { m.Id, Nome = m.Modelo + " (" + m.Matricula + ")" }),
    "Id", "Nome", servico.MotocicletaId);
            ViewBag.Pecas = new SelectList(_context.Pecas, "Id", "Nome");
            ViewBag.PecasData = _context.Pecas.ToDictionary(p => p.Id, p => p.Preco);
            ViewBag.PecasObj = _context.Pecas.ToDictionary(p => p.Id, p => p);
            return View(servico);
        }

        /// <summary>
        /// Processa a edição de um serviço
        /// </summary>
        /// <param name="id">ID do serviço</param>
        /// <param name="servico">Dados do serviço</param>
        /// <param name="pecasIds">IDs das peças</param>
        /// <param name="quantidades">Quantidades das peças</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Mecanico,Root")]
        public async Task<IActionResult> Edit([Bind("Id,Descricao,Data,CustoTotal,ClienteId,MotocicletaId,Status")] Servicos servico)
        {
            if (ModelState.IsValid)
            {
                var servicoAntigo = await _context.Servicos
                    .Include(s => s.ServicoPecas)
                    .FirstOrDefaultAsync(s => s.Id == servico.Id);
                if (servicoAntigo == null)
                {
                    return NotFound();
                }
                if (servicoAntigo.Status == Servicos.ServicoEstado.Concluido || servicoAntigo.Status == Servicos.ServicoEstado.Cancelado)
                {
                    TempData["Error"] = "Não é permitido editar um serviço Concluido ou Cancelado.";
                    return RedirectToAction("Details", new { id = servico.Id });
                }

                // Impede voltar de Concluido ou Cancelado para Pendente
                if ((servicoAntigo.Status == Servicos.ServicoEstado.Concluido || servicoAntigo.Status == Servicos.ServicoEstado.Cancelado) && servico.Status == Servicos.ServicoEstado.Pendente)
                {
                    TempData["Error"] = "Não é permitido alterar o estado de 'Concluido' ou 'Cancelado' para 'Pendente'.";
                    ViewBag.Motocicletas = new SelectList(_context.Motocicletas, "Id", "Modelo", servico.MotocicletaId);
                    return View(servicoAntigo);
                }

                // Só subtrai peças do estoque se mudou de Pendente para Concluido
                if (servicoAntigo.Status == Servicos.ServicoEstado.Pendente && servico.Status == Servicos.ServicoEstado.Concluido)
                {
                    foreach (var sp in servicoAntigo.ServicoPecas)
                    {
                        var peca = await _context.Pecas.FirstOrDefaultAsync(p => p.Id == sp.PecaId);
                        if (peca != null)
                        {
                            if (sp.QuantidadeUsada > peca.QuantidadeEstoque)
                            {
                                TempData["Error"] = $"Stock insuficiente para a peça '{peca.Nome}'. Stock disponível: {peca.QuantidadeEstoque}, necessário: {sp.QuantidadeUsada}.";
                                ViewBag.Motocicletas = new SelectList(_context.Motocicletas, "Id", "Modelo", servico.MotocicletaId);
                                return View(servicoAntigo);
                            }
                            peca.QuantidadeEstoque -= sp.QuantidadeUsada;
                            if (peca.QuantidadeEstoque < 0) peca.QuantidadeEstoque = 0;
                            _context.Pecas.Update(peca);
                        }
                    }
                    await _context.SaveChangesAsync();
                }

                // Atualiza o serviço
                _context.Entry(servicoAntigo).CurrentValues.SetValues(servico);
                await _context.SaveChangesAsync();
                await _hubContext.Clients.All.SendAsync("AtualizarServicos");
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Motocicletas = new SelectList(_context.Motocicletas, "Id", "Modelo", servico.MotocicletaId);
            return View(servico);
        }

        /// <summary>
        /// Exibe o formulário para adicionar uma peça ao serviço
        /// </summary>
        /// <param name="id">ID do serviço</param>
        [Authorize(Roles = "Mecanico,Root")]
        public IActionResult AddPeca(int id)
        {
            var servico = _context.Servicos.Find(id);
            if (servico == null)
                return NotFound();
            if (servico.Status == Servicos.ServicoEstado.Concluido || servico.Status == Servicos.ServicoEstado.Cancelado)
            {
                TempData["Error"] = "Não é permitido adicionar peças a um serviço Concluido ou Cancelado.";
                return RedirectToAction("Details", new { id });
            }
            ViewBag.Pecas = new SelectList(_context.Pecas, "Id", "Nome");
            ViewBag.PecasObj = _context.Pecas.ToDictionary(p => p.Id, p => p);
            return View(new ServicoPecas { ServicoId = id });
        }

        /// <summary>
        /// Processa a adição de uma peça ao serviço
        /// </summary>
        /// <param name="servicoPeca">Dados da relação serviço-peça</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Mecanico,Root")]
        public async Task<IActionResult> AddPeca(ServicoPecas servicoPeca)
        {
            var servico = await _context.Servicos.FindAsync(servicoPeca.ServicoId);
            if (servico == null)
                return NotFound();
            if (servico.Status == Servicos.ServicoEstado.Concluido || servico.Status == Servicos.ServicoEstado.Cancelado)
            {
                TempData["Error"] = "Não é permitido adicionar peças a um serviço Concluido ou Cancelado.";
                return RedirectToAction("Details", new { id = servicoPeca.ServicoId });
            }
            if (ModelState.IsValid && servicoPeca.ServicoId != null)
            {
                var peca = await _context.Pecas.FindAsync(servicoPeca.PecaId);
                if (peca != null)
                {
                    if (servicoPeca.QuantidadeUsada > peca.QuantidadeEstoque)
                    {
                        ModelState.AddModelError("", $"Stock insuficiente para a peça '{peca.Nome}'. Stock disponível: {peca.QuantidadeEstoque}, solicitado: {servicoPeca.QuantidadeUsada}.");
                        ViewBag.Pecas = new SelectList(_context.Pecas, "Id", "Nome", servicoPeca.PecaId);
                        return View(servicoPeca);
                    }
                    peca.QuantidadeEstoque -= servicoPeca.QuantidadeUsada;
                    if (peca.QuantidadeEstoque < 0)
                        peca.QuantidadeEstoque = 0;
                    _context.Pecas.Update(peca);
                }

                // Verifica se já existe a peça neste serviço
                var existente = await _context.ServicoPecas.FirstOrDefaultAsync(sp => sp.ServicoId == servicoPeca.ServicoId && sp.PecaId == servicoPeca.PecaId);
                if (existente != null)
                {
                    existente.QuantidadeUsada += servicoPeca.QuantidadeUsada;
                    _context.ServicoPecas.Update(existente);
                }
                else
                {
                    var ser = new ServicoPecas
                    {
                        ServicoId = servicoPeca.ServicoId,
                        PecaId = servicoPeca.PecaId,
                        QuantidadeUsada = servicoPeca.QuantidadeUsada
                    };
                    _context.ServicoPecas.Add(ser);
                }

                // Atualizar o custo total do serviço
                var servicoDb = await _context.Servicos.FindAsync(servicoPeca.ServicoId);
                if (servicoDb != null && peca != null)
                {
                    servicoDb.CustoTotal += peca.Preco * servicoPeca.QuantidadeUsada;
                    _context.Servicos.Update(servicoDb);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction("Details", new { id = servicoPeca.ServicoId });
            }
            ViewBag.Pecas = new SelectList(_context.Pecas, "Id", "Nome", servicoPeca.PecaId);
            return View(servicoPeca);
        }

        /// <summary>
        /// Exibe o formulário de edição de uma peça no serviço
        /// </summary>
        /// <param name="id">ID da relação serviço-peça</param>
        public async Task<IActionResult> EditPeca(int id)
        {
            var servicoPeca = await _context.ServicoPecas
                .Include(sp => sp.Peca)
                .FirstOrDefaultAsync(sp => sp.Id == id);
            if (servicoPeca == null)
            {
                return NotFound();
            }
            var servicoDb = await _context.Servicos.FindAsync(servicoPeca.ServicoId);
            if (servicoDb == null)
                return NotFound();
            if (servicoDb.Status == Servicos.ServicoEstado.Concluido || servicoDb.Status == Servicos.ServicoEstado.Cancelado)
            {
                TempData["Error"] = "Não é permitido editar peças de um serviço Concluido ou Cancelado.";
                return RedirectToAction("Details", new { id = servicoPeca.ServicoId });
            }
            ViewBag.Pecas = new SelectList(_context.Pecas, "Id", "Nome", servicoPeca.PecaId);
            ViewBag.PecasObj = _context.Pecas.ToDictionary(p => p.Id, p => p);
            return View(servicoPeca);
        }

        /// <summary>
        /// Processa a edição de uma peça no serviço
        /// </summary>
        /// <param name="servicoPeca">Dados da relação serviço-peça</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Mecanico,Root")]
        public async Task<IActionResult> EditPeca(ServicoPecas servicoPeca)
        {
            var servicoDb = await _context.Servicos.FindAsync(servicoPeca.ServicoId);
            if (servicoDb == null)
                return NotFound();
            if (servicoDb.Status == Servicos.ServicoEstado.Concluido || servicoDb.Status == Servicos.ServicoEstado.Cancelado)
            {
                TempData["Error"] = "Não é permitido editar peças de um serviço Concluido ou Cancelado.";
                return RedirectToAction("Details", new { id = servicoPeca.ServicoId });
            }
            if (ModelState.IsValid && servicoPeca.PecaId != null)
            {
                // Buscar o registro antigo
                var antigo = await _context.ServicoPecas.AsNoTracking().FirstOrDefaultAsync(sp => sp.Id == servicoPeca.Id);
                if (antigo != null)
                {
                    var peca = await _context.Pecas.FindAsync(servicoPeca.PecaId);
                    if (peca != null)
                    {
                        // Repor o stock antigo
                        peca.QuantidadeEstoque += antigo.QuantidadeUsada;
                        // Subtrair o novo valor
                        if (servicoPeca.QuantidadeUsada > peca.QuantidadeEstoque)
                        {
                            ModelState.AddModelError("", $"Stock insuficiente para a peça '{peca.Nome}'. Stock disponível: {peca.QuantidadeEstoque}, solicitado: {servicoPeca.QuantidadeUsada}.");
                            ViewBag.Pecas = new SelectList(_context.Pecas, "Id", "Nome", servicoPeca.PecaId);
                            return View(servicoPeca);
                        }
                        peca.QuantidadeEstoque -= servicoPeca.QuantidadeUsada;
                        if (peca.QuantidadeEstoque < 0)
                            peca.QuantidadeEstoque = 0;
                        _context.Pecas.Update(peca);
                    }
                    // Atualizar custo total do serviço
                    var servicoDb2 = await _context.Servicos.FindAsync(servicoPeca.ServicoId);
                    if (servicoDb2 != null && peca != null)
                    {
                        // Remover o valor antigo e somar o novo
                        servicoDb2.CustoTotal -= peca.Preco * antigo.QuantidadeUsada;
                        servicoDb2.CustoTotal += peca.Preco * servicoPeca.QuantidadeUsada;
                        if (servicoDb2.CustoTotal < 0) servicoDb2.CustoTotal = 0;
                        _context.Servicos.Update(servicoDb2);
                    }
                }

                _context.Update(servicoPeca);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", new { id = servicoPeca.ServicoId });
            }
            ViewBag.Pecas = new SelectList(_context.Pecas, "Id", "Nome", servicoPeca.PecaId);
            return View(servicoPeca);
        }

        /// <summary>
        /// Remove uma peça do serviço
        /// </summary>
        /// <param name="id">ID da relação serviço-peça</param>
        [Authorize(Roles = "Mecanico,Root")]
        public async Task<IActionResult> DeletePeca(int id)
        {
            var servicoPeca = await _context.ServicoPecas.FindAsync(id);
            if (servicoPeca == null)
            {
                return NotFound();
            }
            var servicoDb = await _context.Servicos.FindAsync(servicoPeca.ServicoId);
            if (servicoDb == null)
                return NotFound();
            if (servicoDb.Status == Servicos.ServicoEstado.Concluido || servicoDb.Status == Servicos.ServicoEstado.Cancelado)
            {
                TempData["Error"] = "Não é permitido remover peças de um serviço Concluido ou Cancelado.";
                return RedirectToAction("Details", new { id = servicoPeca.ServicoId });
            }
            // Repor o stock da peça
            var peca = await _context.Pecas.FindAsync(servicoPeca.PecaId);
            if (peca != null)
            {
                peca.QuantidadeEstoque += servicoPeca.QuantidadeUsada;
                _context.Pecas.Update(peca);
            }

            // Atualizar custo total do serviço
            var servicoDb2 = await _context.Servicos.FindAsync(servicoPeca.ServicoId);
            if (servicoDb2 != null && peca != null)
            {
                servicoDb2.CustoTotal -= peca.Preco * servicoPeca.QuantidadeUsada;
                if (servicoDb2.CustoTotal < 0) servicoDb2.CustoTotal = 0;
                _context.Servicos.Update(servicoDb2);
            }

            _context.ServicoPecas.Remove(servicoPeca);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { id = servicoPeca.ServicoId });
        }

        /// <summary>
        /// Cria uma encomenda com todas as peças do serviço
        /// </summary>
        /// <param name="id">ID do serviço</param>
        public async Task<IActionResult> EncomendarTodos(int id)
        {
            var servico = await _context.Servicos
                .Include(s => s.ServicoPecas)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (servico == null)
            {
                return NotFound();
            }

            var encomenda = new Encomendas
            {
                DataPedido = DateTime.Now,
                Status = Encomendas.EncomendaEstado.Pendente
            };

            _context.Encomendas.Add(encomenda);
            await _context.SaveChangesAsync();

            foreach (var servicoPeca in servico.ServicoPecas)
            {
                var encomendaPeca = new EncomendaPecas
                {
                    EncomendaId = encomenda.Id,
                    PecaId = servicoPeca.PecaId,
                    Quantidade = servicoPeca.QuantidadeUsada
                };
                _context.EncomendaPecas.Add(encomendaPeca);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Encomendas", new { id = encomenda.Id });
        }

        /// <summary>
        /// Verifica se um serviço existe
        /// </summary>
        /// <param name="id">ID do serviço</param>
        private bool ServicoExists(int id)
        {
            return _context.Servicos.Any(e => e.Id == id);
        }

        /// <summary>
        /// Exibe o formulário de confirmação para remover um serviço
        /// </summary>
        [Authorize(Roles = "Mecanico,Root")]
        public async Task<IActionResult> Delete(int id)
        {
            var servico = await _context.Servicos
                .Include(s => s.ServicoPecas)
                .Include(s => s.Motocicleta)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (servico == null)
            {
                return NotFound();
            }
            ViewBag.HasDependencies = servico.ServicoPecas.Any();
            return View(servico);
        }

        /// <summary>
        /// Processa a remoção de um serviço
        /// </summary>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Mecanico,Root")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var servico = await _context.Servicos
                .Include(s => s.ServicoPecas)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (servico == null)
            {
                return NotFound();
            }
            if (servico.ServicoPecas.Any())
            {
                TempData["Error"] = "Não é possível eliminar um serviço com peças associadas.";
                return RedirectToAction("Details", new { id });
            }
            _context.Servicos.Remove(servico);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Serviço eliminado com sucesso.";
            await _hubContext.Clients.All.SendAsync("AtualizarServicos");
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Obtém as motocicletas de um cliente via AJAX
        /// </summary>
        /// <param name="id">ID do cliente</param>
        [HttpGet]
        public async Task<IActionResult> GetMotocicletasByCliente(int id)
        {
            var motos = await _context.Motocicletas
                .Where(m => m.ClienteId == id)
                .Select(m => new { id = m.Id, matricula = m.Matricula, modelo = m.Modelo })
                .ToListAsync();
            return Json(motos);
        }
    }
}