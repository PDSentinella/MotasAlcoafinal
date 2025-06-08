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
    public class EncomendaPecasController : ControllerBase
    {
        private readonly MotasAlcoaContext _context;

        public EncomendaPecasController(MotasAlcoaContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Devolve a lista com todas as relações entre encomendas e peças
        /// </summary>
        /// <returns>Lista de EncomendaPecas</returns>
        // GET: api/EncomendaPecas
        [HttpGet]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult<IEnumerable<EncomendaPecas>>> GetEncomendaPecas()
        {
            return await _context.EncomendaPecas.ToListAsync();
        }

        /// <summary>
        /// Devolve uma relação específica entre encomenda e peça
        /// </summary>
        /// <param name="id">Identificador da relação EncomendaPeca</param>
        /// <returns>EncomendaPeca específica</returns>
        // GET: api/EncomendaPecas/5
        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult<EncomendaPecas>> GetEncomendaPecas(int id)
        {
            var encomendaPecas = await _context.EncomendaPecas.FindAsync(id);

            if (encomendaPecas == null)
            {
                return NotFound();
            }

            return encomendaPecas;
        }

        /// <summary>
        /// Edita uma relação entre encomenda e peça
        /// </summary>
        /// <param name="id">Identificador da relação a editar</param>
        /// <param name="encomendaPecas">Novos dados da relação EncomendaPeca</param>
        /// <returns>Resultado da operação</returns>
        // PUT: api/EncomendaPecas/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Gestor,Root")]
        public async Task<IActionResult> PutEncomendaPecas(int id, EncomendaPecas encomendaPecas)
        {
            if (id != encomendaPecas.Id)
            {
                return BadRequest();
            }

            _context.Entry(encomendaPecas).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EncomendaPecasExists(id))
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
        /// Adiciona uma nova relação entre encomenda e peça
        /// </summary>
        /// <param name="encomendaPecas">Dados da relação EncomendaPeca a adicionar</param>
        /// <returns>A relação EncomendaPeca criada</returns>
        // POST: api/EncomendaPecas
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Gestor,Root")]
        public async Task<ActionResult<EncomendaPecas>> PostEncomendaPecas(EncomendaPecas encomendaPecas)
        {
            _context.EncomendaPecas.Add(encomendaPecas);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEncomendaPecas", new { id = encomendaPecas.Id }, encomendaPecas);
        }

        /// <summary>
        /// Remove uma relação entre encomenda e peça
        /// </summary>
        /// <param name="id">Identificador da relação a remover</param>
        /// <returns>Resultado da operação</returns>
        // DELETE: api/EncomendaPecas/5
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Gestor,Root")]
        public async Task<IActionResult> DeleteEncomendaPecas(int id)
        {
            var encomendaPecas = await _context.EncomendaPecas.FindAsync(id);
            if (encomendaPecas == null)
            {
                return NotFound();
            }

            _context.EncomendaPecas.Remove(encomendaPecas);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Verifica se uma relação EncomendaPeca existe
        /// </summary>
        /// <param name="id">Identificador da relação a verificar</param>
        /// <returns>'true' se a relação existe, senão 'false'</returns>
        private bool EncomendaPecasExists(int id)
        {
            return _context.EncomendaPecas.Any(e => e.Id == id);
        }
    }
}