using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MotasAlcoafinal.Models;
using motasAlcoafinal.Models;
using Microsoft.AspNetCore.Authorization;

namespace MotasAlcoafinal.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class ServicosController : ControllerBase
    {
        private readonly MotasAlcoaContext _context;

        public ServicosController(MotasAlcoaContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Devolve a lista com todos os serviços
        /// </summary>
        /// <returns>Lista de serviços</returns>
        /// <response code="200">Lista de serviços obtida com sucesso</response>
        /// <response code="401">Utilizador não autenticado</response>
        // GET: api/Servicos
        [HttpGet]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult<IEnumerable<Servicos>>> GetServicos()
        {
            return await _context.Servicos.ToListAsync();
        }

        /// <summary>
        /// Devolve um serviço específico pelo seu identificador
        /// </summary>
        /// <param name="id">Identificador do serviço</param>
        /// <returns>Serviço solicitado</returns>
        /// <response code="200">Serviço encontrado</response>
        /// <response code="404">Serviço não encontrado</response>
        /// <response code="401">Utilizador não autenticado</response>
        // GET: api/Servicos/5
        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult<Servicos>> GetServicos(int id)
        {
            var servicos = await _context.Servicos.FindAsync(id);

            if (servicos == null)
            {
                return NotFound();
            }

            return servicos;
        }

        /// <summary>
        /// Edita um serviço existente
        /// </summary>
        /// <param name="id">Identificador do serviço a editar</param>
        /// <param name="servicos">Dados atualizados do serviço</param>
        /// <returns>Resultado da operação de edição</returns>
        /// <response code="204">Serviço editado com sucesso</response>
        /// <response code="400">Dados inválidos fornecidos</response>
        /// <response code="404">Serviço não encontrado</response>
        /// <response code="401">Utilizador não autenticado</response>
        /// <response code="403">Utilizador sem permissões (requer role Mecanico ou Root)</response>
        // PUT: api/Servicos/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Mecanico,Root")]
        public async Task<IActionResult> PutServicos(int id, Servicos servicos)
        {
            if (id != servicos.Id)
            {
                return BadRequest();
            }

            _context.Entry(servicos).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServicosExists(id))
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
        /// Cria um novo serviço
        /// </summary>
        /// <param name="servicos">Dados do serviço a criar</param>
        /// <returns>O serviço criado</returns>
        /// <response code="201">Serviço criado com sucesso</response>
        /// <response code="400">Dados inválidos fornecidos</response>
        /// <response code="401">Utilizador não autenticado</response>
        /// <response code="403">Utilizador sem permissões (requer role Mecanico ou Root)</response>
        // POST: api/Servicos
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Mecanico,Root")]
        public async Task<ActionResult<Servicos>> PostServicos(Servicos servicos)
        {
            _context.Servicos.Add(servicos);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetServicos", new { id = servicos.Id }, servicos);
        }

        /// <summary>
        /// Remove um serviço
        /// </summary>
        /// <param name="id">Identificador do serviço a remover</param>
        /// <returns>Resultado da operação de remoção</returns>
        /// <response code="204">Serviço removido com sucesso</response>
        /// <response code="404">Serviço não encontrado</response>
        /// <response code="401">Utilizador não autenticado</response>
        /// <response code="403">Utilizador sem permissões (requer role Mecanico ou Root)</response>
        // DELETE: api/Servicos/5
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Mecanico,Root")]
        public async Task<IActionResult> DeleteServicos(int id)
        {
            var servicos = await _context.Servicos.FindAsync(id);
            if (servicos == null)
            {
                return NotFound();
            }

            _context.Servicos.Remove(servicos);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Verifica se um serviço existe
        /// </summary>
        /// <param name="id">Identificador do serviço a verificar</param>
        /// <returns>'true' se o serviço existe, senão 'false'</returns>
        private bool ServicosExists(int id)
        {
            return _context.Servicos.Any(e => e.Id == id);
        }
    }
}