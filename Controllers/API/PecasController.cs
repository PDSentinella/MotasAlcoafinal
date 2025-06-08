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
    public class PecasController : ControllerBase
    {
        private readonly MotasAlcoaContext _context;

        public PecasController(MotasAlcoaContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Devolve a lista com todas as peças
        /// </summary>
        /// <returns>Lista de peças</returns>
        /// <response code="200">Lista de peças obtida com sucesso</response>
        /// <response code="401">Utilizador não autenticado</response>
        // GET: api/Pecas
        [HttpGet]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult<IEnumerable<Pecas>>> GetPecas()
        {
            return await _context.Pecas.ToListAsync();
        }

        /// <summary>
        /// Devolve uma peça específica pelo seu identificador
        /// </summary>
        /// <param name="id">Identificador da peça</param>
        /// <returns>Peça solicitada</returns>
        /// <response code="200">Peça encontrada</response>
        /// <response code="404">Peça não encontrada</response>
        /// <response code="401">Utilizador não autenticado</response>
        // GET: api/Pecas/5
        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult<Pecas>> GetPecas(int id)
        {
            var pecas = await _context.Pecas.FindAsync(id);

            if (pecas == null)
            {
                return NotFound();
            }

            return pecas;
        }

        /// <summary>
        /// Edita uma peça existente
        /// </summary>
        /// <param name="id">Identificador da peça a editar</param>
        /// <param name="pecas">Dados atualizados da peça</param>
        /// <returns>Resultado da operação de edição</returns>
        /// <response code="204">Peça editada com sucesso</response>
        /// <response code="400">Dados inválidos fornecidos</response>
        /// <response code="404">Peça não encontrada</response>
        /// <response code="401">Utilizador não autenticado</response>
        // PUT: api/Pecas/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> PutPecas(int id, Pecas pecas)
        {
            if (id != pecas.Id)
            {
                return BadRequest();
            }

            _context.Entry(pecas).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PecasExists(id))
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
        /// Cria uma nova peça
        /// </summary>
        /// <param name="pecas">Dados da peça a criar</param>
        /// <returns>A peça criada</returns>
        /// <response code="201">Peça criada com sucesso</response>
        /// <response code="400">Dados inválidos fornecidos</response>
        /// <response code="401">Utilizador não autenticado</response>
        // POST: api/Pecas
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult<Pecas>> PostPecas(Pecas pecas)
        {
            _context.Pecas.Add(pecas);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPecas", new { id = pecas.Id }, pecas);
        }

        /// <summary>
        /// Remove uma peça
        /// </summary>
        /// <param name="id">Identificador da peça a remover</param>
        /// <returns>Resultado da operação de remoção</returns>
        /// <response code="204">Peça removida com sucesso</response>
        /// <response code="404">Peça não encontrada</response>
        /// <response code="401">Utilizador não autenticado</response>
        // DELETE: api/Pecas/5
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> DeletePecas(int id)
        {
            var pecas = await _context.Pecas.FindAsync(id);
            if (pecas == null)
            {
                return NotFound();
            }

            _context.Pecas.Remove(pecas);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Verifica se uma peça existe
        /// </summary>
        /// <param name="id">Identificador da peça a verificar</param>
        /// <returns>'true' se a peça existe, senão 'false'</returns>
        private bool PecasExists(int id)
        {
            return _context.Pecas.Any(e => e.Id == id);
        }
    }
}