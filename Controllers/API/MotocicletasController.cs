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
    public class MotocicletasController : ControllerBase
    {
        private readonly MotasAlcoaContext _context;

        public MotocicletasController(MotasAlcoaContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Devolve a lista com todas as motocicletas
        /// </summary>
        /// <returns>Lista de motocicletas ordenadas por ano decrescente</returns>
        /// <response code="200">Lista de motocicletas obtida com sucesso</response>
        /// <response code="401">Utilizador não autenticado</response>
        // GET: api/Motocicletas
        [HttpGet]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult<IEnumerable<MotocicletasDTO>>> GetMotocicletas()
        {
            var listagemFotos = await _context.Motocicletas
                .OrderByDescending(m=>m.Ano)
                .Select(m=> new MotocicletasDTO
                {
                    Marca=m.Marca,
                    Modelo=m.Modelo,
                    Ano=m.Ano,
                })
                .ToListAsync();

            return listagemFotos;
        }

        /// <summary>
        /// Devolve uma motocicleta específica pelo seu identificador
        /// </summary>
        /// <param name="id">Identificador da motocicleta</param>
        /// <returns>Motocicleta solicitada</returns>
        /// <response code="200">Motocicleta encontrada</response>
        /// <response code="404">Motocicleta não encontrada</response>
        /// <response code="401">Utilizador não autenticado</response>
        // GET: api/Motocicletas/5
        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult<MotocicletasDTO>> GetMotocicleta(int id)
        {
            var motocicleta = await _context.Motocicletas
                                            .Where(m=>m.Id == id)
                                            .Select(m=> new MotocicletasDTO{
                                             Marca = m.Marca,
                                             Modelo=m.Modelo,
                                             Ano=m.Ano,
                                            }).FirstOrDefaultAsync();

            if (motocicleta == null)
            {
                return NotFound();
            }

            return motocicleta;
        }

        /// <summary>
        /// Edita uma motocicleta existente
        /// </summary>
        /// <param name="id">Identificador da motocicleta a editar</param>
        /// <param name="motocicleta">Dados atualizados da motocicleta</param>
        /// <returns>Resultado da operação de edição</returns>
        /// <response code="204">Motocicleta editada com sucesso</response>
        /// <response code="400">Dados inválidos fornecidos</response>
        /// <response code="404">Motocicleta não encontrada</response>
        /// <response code="401">Utilizador não autenticado</response>
        /// <response code="403">Utilizador sem permissões (requer role Mecanico ou Root)</response>
        // PUT: api/Motocicletas/5
        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Mecanico,Root")]
        public async Task<IActionResult> PutMotocicleta(int id, Motocicletas motocicleta)
        {
            if (id != motocicleta.Id)
            {
                return BadRequest();
            }

            _context.Entry(motocicleta).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MotocicletasExists(id))
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
        /// Cria uma nova motocicleta
        /// </summary>
        /// <param name="motocicleta">Dados da motocicleta a criar</param>
        /// <returns>A motocicleta criada</returns>
        /// <response code="201">Motocicleta criada com sucesso</response>
        /// <response code="400">Dados inválidos fornecidos</response>
        /// <response code="401">Utilizador não autenticado</response>
        /// <response code="403">Utilizador sem permissões (requer role Mecanico ou Root)</response>
        // POST: api/Motocicletas
        [HttpPost]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Mecanico,Root")]
        public async Task<ActionResult<Motocicletas>> PostMotocicleta(Motocicletas motocicleta)
        {
            _context.Motocicletas.Add(motocicleta);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMotocicletas", new { id = motocicleta.Id }, motocicleta);
        }

        /// <summary>
        /// Remove uma motocicleta
        /// </summary>
        /// <param name="id">Identificador da motocicleta a remover</param>
        /// <returns>Resultado da operação de remoção</returns>
        /// <response code="204">Motocicleta removida com sucesso</response>
        /// <response code="404">Motocicleta não encontrada</response>
        /// <response code="401">Utilizador não autenticado</response>
        /// <response code="403">Utilizador sem permissões (requer role Mecanico ou Root)</response>
        // DELETE: api/Motocicletas/5
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Mecanico,Root")]
        public async Task<IActionResult> DeleteMotocicleta(int id)
        {
            var motocicleta = await _context.Motocicletas.FindAsync(id);
            if (motocicleta == null)
            {
                return NotFound();
            }

            _context.Motocicletas.Remove(motocicleta);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Verifica se uma motocicleta existe
        /// </summary>
        /// <param name="id">Identificador da motocicleta a verificar</param>
        /// <returns>'true' se a motocicleta existe, senão 'false'</returns>
        private bool MotocicletasExists(int id)
        {
            return _context.Motocicletas.Any(e => e.Id == id);
        }
    }
}