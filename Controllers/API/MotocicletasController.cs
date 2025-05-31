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

namespace MotasAlcoafinal.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class MotocicletasController : ControllerBase
    {
        private readonly MotasAlcoaContext _context;

        public MotocicletasController(MotasAlcoaContext context)
        {
            _context = context;
        }

        // GET: api/Motocicletas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MotocicletasDTO>>> GetMotocicletas()
        {

            //return await _context.Motocicletas.ToListAsync();

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

        // GET: api/Motocicletas/5
        [HttpGet("{id}")]
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

        // PUT: api/Motocicletas/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
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

        // POST: api/Motocicletas
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Motocicletas>> PostMotocicleta(Motocicletas motocicleta)
        {
            _context.Motocicletas.Add(motocicleta);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMotocicletas", new { id = motocicleta.Id }, motocicleta);
        }

        // DELETE: api/Motocicletas/5
        [HttpDelete("{id}")]
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

        private bool MotocicletasExists(int id)
        {
            return _context.Motocicletas.Any(e => e.Id == id);
        }
    }
}
