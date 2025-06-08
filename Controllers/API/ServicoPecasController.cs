using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MotasAlcoafinal.Models;
using motasAlcoafinal.Models;

namespace MotasAlcoafinal.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServicoPecasController : ControllerBase
    {
        private readonly MotasAlcoaContext _context;

        public ServicoPecasController(MotasAlcoaContext context)
        {
            _context = context;
        }

        // GET: api/ServicoPecas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ServicoPecas>>> GetServicoPecas()
        {
            return await _context.ServicoPecas.ToListAsync();
        }

        // GET: api/ServicoPecas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ServicoPecas>> GetServicoPecas(int id)
        {
            var servicoPecas = await _context.ServicoPecas.FindAsync(id);

            if (servicoPecas == null)
            {
                return NotFound();
            }

            return servicoPecas;
        }

        // PUT: api/ServicoPecas/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutServicoPecas(int id, ServicoPecas servicoPecas)
        {
            if (id != servicoPecas.Id)
            {
                return BadRequest();
            }

            _context.Entry(servicoPecas).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServicoPecasExists(id))
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

        // POST: api/ServicoPecas
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ServicoPecas>> PostServicoPecas(ServicoPecas servicoPecas)
        {
            _context.ServicoPecas.Add(servicoPecas);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetServicoPecas", new { id = servicoPecas.Id }, servicoPecas);
        }

        // DELETE: api/ServicoPecas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteServicoPecas(int id)
        {
            var servicoPecas = await _context.ServicoPecas.FindAsync(id);
            if (servicoPecas == null)
            {
                return NotFound();
            }

            _context.ServicoPecas.Remove(servicoPecas);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ServicoPecasExists(int id)
        {
            return _context.ServicoPecas.Any(e => e.Id == id);
        }
    }
}
