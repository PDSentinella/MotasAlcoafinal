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
    public class EncomendasController : ControllerBase
    {
        private readonly MotasAlcoaContext _context;

        public EncomendasController(MotasAlcoaContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Devolve a lista com todas as encomendas
        /// </summary>
        /// <returns>Lista de encomendas</returns>
        // GET: api/Encomendas
        [HttpGet]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult<IEnumerable<Encomendas>>> GetEncomendas()
        {
            return await _context.Encomendas.ToListAsync();
        }

        /// <summary>
        /// Devolve uma encomenda específica pelo seu identificador
        /// </summary>
        /// <param name="id">Identificador da encomenda</param>
        /// <returns>Encomenda solicitada</returns>
        /// <response code="200">Encomenda encontrada</response>
        /// <response code="404">Encomenda não encontrada</response>
        // GET: api/Encomendas/5
        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult<Encomendas>> GetEncomendas(int id)
        {
            var encomendas = await _context.Encomendas.FindAsync(id);

            if (encomendas == null)
            {
                return NotFound();
            }

            return encomendas;
        }

        /// <summary>
        /// Edita uma encomenda existente
        /// </summary>
        /// <param name="id">Identificador da encomenda a editar</param>
        /// <param name="encomendas">Dados atualizados da encomenda</param>
        /// <returns>Resultado da operação de edição</returns>
        /// <response code="204">Encomenda editada com sucesso</response>
        /// <response code="400">Dados inválidos fornecidos</response>
        /// <response code="404">Encomenda não encontrada</response>
        // PUT: api/Encomendas/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Gestor,Root")]
        public async Task<IActionResult> PutEncomendas(int id, Encomendas encomendas)
        {
            if (id != encomendas.Id)
            {
                return BadRequest();
            }

            _context.Entry(encomendas).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EncomendasExists(id))
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
        /// Cria uma nova encomenda
        /// </summary>
        /// <param name="encomendas">Dados da encomenda a criar</param>
        /// <returns>A encomenda criada</returns>
        /// <response code="201">Encomenda criada com sucesso</response>
        /// <response code="400">Dados inválidos fornecidos</response>
        // POST: api/Encomendas
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Gestor,Root")]
        public async Task<ActionResult<Encomendas>> PostEncomendas(Encomendas encomendas)
        {
            _context.Encomendas.Add(encomendas);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEncomendas", new { id = encomendas.Id }, encomendas);
        }

        /// <summary>
        /// Remove uma encomenda
        /// </summary>
        /// <param name="id">Identificador da encomenda a remover</param>
        /// <returns>Resultado da operação de remoção</returns>
        /// <response code="204">Encomenda removida com sucesso</response>
        /// <response code="404">Encomenda não encontrada</response>
        // DELETE: api/Encomendas/5
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Gestor,Root")]
        public async Task<IActionResult> DeleteEncomendas(int id)
        {
            var encomendas = await _context.Encomendas.FindAsync(id);
            if (encomendas == null)
            {
                return NotFound();
            }

            _context.Encomendas.Remove(encomendas);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Verifica se uma encomenda existe
        /// </summary>
        /// <param name="id">Identificador da encomenda a verificar</param>
        /// <returns>'true' se a encomenda existe, senão 'false'</returns>
        private bool EncomendasExists(int id)
        {
            return _context.Encomendas.Any(e => e.Id == id);
        }
    }
}