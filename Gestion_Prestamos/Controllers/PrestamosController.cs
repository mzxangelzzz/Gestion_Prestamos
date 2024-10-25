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
    public class PrestamosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PrestamosController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Prestamo>>> GetPrestamos()
        {
            return await _context.gep_prestamo.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Prestamo>> GetPrestamo(int id)
        {
            var prestamo = await _context.gep_prestamo.FindAsync(id);

            if (prestamo == null)
            {
                return NotFound();
            }

            return prestamo;
        }

        [HttpGet("{id}/finiquito")]
        public async Task<ActionResult<FiniquitoResult>> GetFiniquito(int id)
        {
            try
            {
                var result = await (from p in _context.gep_prestamo
                                    join c in _context.gep_clientes on p.pre_id_cliente equals c.id_cliente
                                    join pg in _context.gep_pagos_prestamos on p.id_prestamo equals pg.pag_id_prestamo into pagos
                                    from pago in pagos.DefaultIfEmpty()
                                    join cap in _context.gep_cancelacion_anticipada_prestamo on p.id_prestamo equals cap.cap_prestamo into cancelaciones
                                    from cancelacion in cancelaciones.DefaultIfEmpty()
                                    where p.id_prestamo == id && p.pre_estado == true
                                    group new { pago, cancelacion } by new
                                    {
                                        p.id_prestamo,
                                        c.cli_nombre,
                                        c.cli_apellido,
                                        p.pre_monto_prestamo,
                                        p.pre_saldo_restante
                                    } into g
                                    select new FiniquitoResult
                                    {
                                        IdPrestamo = g.Key.id_prestamo,
                                        ClienteNombre = g.Key.cli_nombre,
                                        ClienteApellido = g.Key.cli_apellido,
                                        MontoPrestamo = g.Key.pre_monto_prestamo,
                                        SaldoRestante = g.Key.pre_saldo_restante,
                                        TotalPagado = g.Sum(x => x.pago != null ? x.pago.pag_total_pagado : 0),  // Reemplazado por pag_total_pagado
                                        CancelacionAnticipada = g.Max(x => x.cancelacion != null ? x.cancelacion.cap_monto_cancelado : 0),
                                        Penalidad = g.Max(x => x.cancelacion != null ? x.cancelacion.cap_penalidad : 0),
                                        Finiquito = g.Key.pre_saldo_restante
                                                    - g.Sum(x => x.pago != null ? x.pago.pag_total_pagado : 0)  // Reemplazado por pag_total_pagado
                                                    - g.Max(x => x.cancelacion != null ? x.cancelacion.cap_monto_cancelado : 0)
                                                    + g.Max(x => x.cancelacion != null ? x.cancelacion.cap_penalidad : 0)
                                    }).FirstOrDefaultAsync();

                if (result == null)
                {
                    return NotFound("Préstamo no encontrado o no activo.");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al calcular el finiquito: {ex.Message}");
            }
        }

        [HttpGet("cliente/{id_cliente}")]
        public async Task<ActionResult<IEnumerable<Prestamo>>> GetPrestamosByCliente(int id_cliente)
        {
            try
            {
                // Busca los préstamos asociados al cliente
                var prestamos = await _context.gep_prestamo
                                              .Where(p => p.pre_id_cliente == id_cliente)
                                              .ToListAsync();

                // Si no hay préstamos para ese cliente
                if (prestamos == null || prestamos.Count == 0)
                {
                    return NotFound("No se encontraron préstamos activos para este cliente.");
                }

                return Ok(prestamos);
            }
            catch (Exception ex)
            {
                // Devuelve un error si algo falla en la consulta
                return StatusCode(500, $"Error al obtener los préstamos del cliente: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Prestamo>> PostPrestamo(Prestamo prestamo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                prestamo.pre_estado = true; // Activo
                prestamo.pre_fecha_creacion = DateTime.UtcNow;

                if (prestamo.pre_estado_prestamo == "Activo")
                {
                    prestamo.pre_aprobacion = true;
                }

                if (prestamo.pre_aprobacion == true)
                {
                    prestamo.pre_fecha_aprobacion = DateTime.UtcNow;
                    prestamo.pre_fecha_pago_cuotas = DateTime.UtcNow.AddMonths(prestamo.pre_plazo_prestamo);
                }

                // Calcular pre_monto_cuotas
                if (prestamo.pre_plazo_prestamo > 0)
                {
                    decimal montoCuotaMensual = (prestamo.pre_saldo_restante / prestamo.pre_plazo_prestamo) / 12;
                    prestamo.pre_monto_cuotas = montoCuotaMensual;
                }
                else
                {
                    prestamo.pre_monto_cuotas = 0; // O manejar un valor por defecto si el plazo es 0 o inválido.
                }

                _context.gep_prestamo.Add(prestamo);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetPrestamo), new { id = prestamo.id_prestamo }, prestamo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ocurrió un error al guardar el préstamo: {ex.Message}");
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> PutPrestamo(int id, Prestamo prestamo)
        {
            if (id != prestamo.id_prestamo)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                prestamo.pre_fecha_edicion = DateTime.UtcNow;
                if (prestamo.pre_estado_prestamo == "Activo")
                {
                    prestamo.pre_aprobacion = true;
                }
                if (prestamo.pre_aprobacion == true)
                {
                    prestamo.pre_fecha_aprobacion = DateTime.UtcNow;
                    prestamo.pre_fecha_pago_cuotas = DateTime.UtcNow.AddMonths(prestamo.pre_plazo_prestamo);
                }

                _context.Entry(prestamo).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok(new { Message = "Préstamo actualizado exitosamente" });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PrestamoExists(id))
                {
                    return NotFound();
                }
                throw;
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ocurrió un error al actualizar el préstamo: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePrestamo(int id)
        {
            try
            {
                var prestamo = await _context.gep_prestamo.FindAsync(id);
                if (prestamo == null)
                {
                    return NotFound();
                }

                prestamo.pre_estado = false;
                prestamo.pre_fecha_eliminacion = DateTime.UtcNow;
                _context.Entry(prestamo).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok(new { Message = "Préstamo eliminado exitosamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ocurrió un error al eliminar el préstamo: {ex.Message}");
            }
        }

        private bool PrestamoExists(int id)
        {
            return _context.gep_prestamo.Any(e => e.id_prestamo == id);
        }
    }
}
