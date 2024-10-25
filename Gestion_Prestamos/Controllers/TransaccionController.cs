using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Gestion_Prestamos.Data;
using Gestion_Prestamos.Models;

namespace Gestion_Prestamos.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransaccionesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TransaccionesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/transacciones
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Transaccion>>> GetTransacciones()
        {
            return await _context.gep_transaccion.ToListAsync();
        }

        // GET: api/transacciones/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Transaccion>> GetTransaccion(int id)
        {
            var transaccion = await _context.gep_transaccion.FindAsync(id);

            if (transaccion == null)
            {
                return NotFound();
            }

            return transaccion;
        }

        [HttpPost]
        public async Task<ActionResult<Transaccion>> PostTransaccion(Transaccion transaccion)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    transaccion.tra_fecha_creacion = DateTime.UtcNow;

                    _context.gep_transaccion.Add(transaccion);
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();

                    return CreatedAtAction(nameof(GetTransaccion), new { id = transaccion.tra_no_transaccion }, new { Message = "Transacción agregada exitosamente" });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(500, "Ocurrió un error al guardar la transacción: " + ex.Message);
                }
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutTransaccion(int id, Transaccion transaccion)
        {
            if (id != transaccion.tra_no_transaccion)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    transaccion.tra_fecha_edicion = DateTime.UtcNow; // Actualizar fecha de edición
                    _context.Entry(transaccion).State = EntityState.Modified;
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();

                    return Ok(new { Message = "Transacción actualizada exitosamente" });
                }
                catch (DbUpdateConcurrencyException)
                {
                    await transaction.RollbackAsync();

                    if (!TransaccionExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(500, "Ocurrió un error al actualizar la transacción: " + ex.Message);
                }
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransaccion(int id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var transaccion = await _context.gep_transaccion.FindAsync(id);
                    if (transaccion == null)
                    {
                        return NotFound();
                    }

                    transaccion.tra_estado = false;
                    transaccion.tra_fecha_eliminacion = DateTime.UtcNow; // Actualizar fecha de eliminación
                    _context.Entry(transaccion).State = EntityState.Modified;
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();

                    return Ok(new { Message = "Transacción eliminada exitosamente" });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(500, "Ocurrió un error al eliminar la transacción: " + ex.Message);
                }
            }
        }

        private bool TransaccionExists(int id)
        {
            return _context.gep_transaccion.Any(e => e.tra_no_transaccion == id);
        }
    }
}
