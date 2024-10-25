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
    public class CancelacionAnticipadaPrestamoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CancelacionAnticipadaPrestamoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/cancelacionanticipadaprestamo
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CancelacionAnticipadaPrestamo>>> GetCancelacionAnticipadaPrestamos()
        {
            return await _context.gep_cancelacion_anticipada_prestamo.ToListAsync();
        }

        // GET: api/cancelacionanticipadaprestamo/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CancelacionAnticipadaPrestamo>> GetCancelacionAnticipadaPrestamo(int id)
        {
            var cancelacion = await _context.gep_cancelacion_anticipada_prestamo.FindAsync(id);

            if (cancelacion == null)
            {
                return NotFound();
            }

            return cancelacion;
        }

        // POST: api/cancelacionanticipadaprestamo
        [HttpPost]
        public async Task<ActionResult<CancelacionAnticipadaPrestamo>> PostCancelacionAnticipadaPrestamo(CancelacionAnticipadaPrestamo cancelacion)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var prestamo = await _context.gep_prestamo.FindAsync(cancelacion.cap_prestamo);
            if (prestamo == null || prestamo.pre_estado_prestamo != "Activo")
            {
                return NotFound("Préstamo no encontrado o no está activo para cancelación");
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Rellenar los datos automáticamente
                    cancelacion.cap_fecha_cancelacion = DateTime.UtcNow; // Fecha de cancelación
                    cancelacion.cap_monto_cancelado = prestamo.pre_saldo_restante; // Monto cancelado
                    cancelacion.cap_estado = true; // Estado de la cancelación
                    cancelacion.cap_fecha_creacion = DateTime.UtcNow; // Fecha de creación de la cancelación

                    // Guardar la cancelación anticipada
                    _context.gep_cancelacion_anticipada_prestamo.Add(cancelacion);
                    await _context.SaveChangesAsync();

                    // Actualizar el estado del préstamo a cancelado
                    prestamo.pre_estado_prestamo = "Cancelado";
                    prestamo.pre_saldo_restante = 0; // Resetear saldo
                    _context.Entry(prestamo).State = EntityState.Modified;
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();

                    return CreatedAtAction(nameof(GetCancelacionAnticipadaPrestamo), new { id = cancelacion.cap_id_cancelacion }, new { Message = "Cancelación anticipada agregada exitosamente" });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(500, "Ocurrió un error al guardar la cancelación anticipada: " + ex.Message);
                }
            }
        }


        /* PUT: api/cancelacionanticipadaprestamo/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCancelacionAnticipadaPrestamo(int id, CancelacionAnticipadaPrestamo cancelacion)
        {
            if (id != cancelacion.cap_id_cancelacion)
            {
                return BadRequest("El ID proporcionado no coincide con el de la cancelación anticipada.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Verificar si el préstamo relacionado existe y está activo
            var prestamo = await _context.gep_prestamo.FindAsync(cancelacion.cap_prestamo);
            if (prestamo == null || prestamo.pre_estado_prestamo != "Cancelado")
            {
                return NotFound("Préstamo no encontrado o no está en estado de 'Cancelado' para editar la cancelación.");
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Rellenar campos automáticos de edición
                    cancelacion.cap_fecha_edicion = DateTime.UtcNow; // Fecha de la edición

                    // Actualizar la cancelación anticipada
                    _context.Entry(cancelacion).State = EntityState.Modified;
                    await _context.SaveChangesAsync();

                    // (Opcional) Si hay que hacer algún ajuste en el préstamo, como modificar el saldo restante
                    // prestamo.pre_saldo_restante = [nuevo_saldo];
                    // _context.Entry(prestamo).State = EntityState.Modified;
                    // await _context.SaveChangesAsync();

                    await transaction.CommitAsync();

                    return Ok(new { Message = "Cancelación anticipada actualizada exitosamente" });
                }
                catch (DbUpdateConcurrencyException)
                {
                    await transaction.RollbackAsync();

                    if (!CancelacionAnticipadaPrestamoExists(id))
                    {
                        return NotFound("La cancelación anticipada con el ID proporcionado no existe.");
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(500, "Ocurrió un error al actualizar la cancelación anticipada: " + ex.Message);
                }
            }
        }


        // DELETE: api/cancelacionanticipadaprestamo/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCancelacionAnticipadaPrestamo(int id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var cancelacion = await _context.gep_cancelacion_anticipada_prestamo.FindAsync(id);
                    if (cancelacion == null)
                    {
                        return NotFound();
                    }

                    cancelacion.cap_estado = false;
                    cancelacion.cap_fecha_eliminacion = DateTime.UtcNow;
                    _context.Entry(cancelacion).State = EntityState.Modified;
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();

                    return Ok(new { Message = "Cancelación anticipada eliminada exitosamente" });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(500, "Ocurrió un error al eliminar la cancelación anticipada: " + ex.Message);
                }
            }
        }
        */

        private bool CancelacionAnticipadaPrestamoExists(int id)
        {
            return _context.gep_cancelacion_anticipada_prestamo.Any(e => e.cap_id_cancelacion == id);
        }
    }
}
