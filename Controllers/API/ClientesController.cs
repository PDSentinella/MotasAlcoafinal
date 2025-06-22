using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MotasAlcoafinal.Models;
using motasAlcoafinal.Models;
using MotasAlcoafinal.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace MotasAlcoafinal.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class ClientesController : ControllerBase
    {
        private readonly MotasAlcoaContext _context;

        public ClientesController(MotasAlcoaContext context)
        {
            _context = context;
        }


        /// <summary>
        /// Devolve a lista com todos os clientes
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult<IEnumerable<ClientesDTO>>> GetClientes()
        {

            var listagemClientes = await _context.Clientes
                                   .OrderByDescending(c => c.Nome)
                                   .Select(c => new ClientesDTO
                                   {
                                       Nome = c.Nome,
                                       Email = c.Email,
                                       Motocicletas = c.Motocicletas.Select(m => new MotocicletasDTO
                                       {
                                           Marca = m.Marca,
                                           Modelo = m.Modelo,
                                           Ano = m.Ano
                                       }).ToList()

                                   })
                                   .ToListAsync();

            return listagemClientes;
        }


        /// <summary>
        /// Devolve um cliente específico pelo seu identificador
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/Clientes/5
        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult<ClientesDTO>> GetCliente(int id)
        {
            var cliente = await _context.Clientes
                                        .Where(c=>c.Id == id)
                                        .Select(c => new ClientesDTO
                                        {
                                            Nome = c.Nome,
                                            Email = c.Email,
                                            Motocicletas = c.Motocicletas.Select(m => new MotocicletasDTO
                                            {
                                                Marca = m.Marca,
                                                Modelo = m.Modelo,
                                                Ano= m.Ano
                                            }).ToList()
                                        }).FirstOrDefaultAsync();

            if (cliente == null)
            {
                return NotFound();
            }

            return cliente;
        }


        /// <summary>
        /// Edita um cliente existente
        /// </summary>
        /// <param name="id"> identificação do cliente a editar</param>
        /// <param name="cliente">Novos dados do cliente</param>
        /// <returns></returns>
        // PUT: api/Clientes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Mecanico,Root")]
        public async Task<IActionResult> PutCliente(int id, Clientes cliente)
        {
            if (id != cliente.Id)
            {
                return BadRequest();
            }

            _context.Entry(cliente).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClienteExiste(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        /// <summary>
        /// Cria um novo cliente
        /// </summary>
        /// <param name="cliente">Dados do cliente a adicionar</param>
        /// <returns></returns>
        // POST: api/Clientes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Mecanico,Root")]
        public async Task<ActionResult<ClientesDTO>> PostCliente(ClientesCreateDTO dto)
        {
            var cliente = new Clientes
            {
                Nome = dto.Nome,
                Telefone = dto.Telefone,
                Email = dto.Email,
                Endereco = dto.Endereco
            };

            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            var clienteDTO = new ClientesDTO
            {
                Nome = cliente.Nome,
                Email = cliente.Email,
                Motocicletas = new List<MotocicletasDTO>()
            };

            return CreatedAtAction(nameof(GetCliente), new { id = cliente.Id }, clienteDTO);
        }

        /// <summary>
        /// Apaga um cliente
        /// </summary>
        /// <param name="id">Identificador do cliente a apagar</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Mecanico,Root")]
        public async Task<IActionResult> DeleteCliente(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }

            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Determina se a categoria existe
        /// </summary>
        /// <param name="id">Identificador do cliente a procurar</param>
        /// <returns>'true' se o cliente existe, senão 'false'</returns>
        private bool ClienteExiste(int id)
        {
            return _context.Clientes.Any(e => e.Id == id);
        }
    }
}