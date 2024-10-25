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
    public class PagosPrestamosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PagosPrestamosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/pagosprestamos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PagosPrestamos>>> GetPagosPrestamos()
        {
            return await _context.gep_pagos_prestamos.ToListAsync();
        }

        // GET: api/pagosprestamos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PagosPrestamos>> GetPagosPrestamos(int id)
        {
            var pagosPrestamos = await _context.gep_pagos_prestamos.FindAsync(id);

            if (pagosPrestamos == null)
            {
                return NotFound();
            }

            return pagosPrestamos;
        }

        [HttpPost]
        public async Task<ActionResult<PagosPrestamos>> PostPagosPrestamos(PagosPrestamos pagosPrestamos)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Calcular pag_total_pagado
            pagosPrestamos.pag_total_pagado = pagosPrestamos.pag_monto_cuota + pagosPrestamos.pag_mora;

            pagosPrestamos.pag_estado = true;
            pagosPrestamos.pag_fecha_creacion = DateTime.UtcNow;
            pagosPrestamos.pag_fecha_pago = DateTime.UtcNow;

            // Guardar el pago
            _context.gep_pagos_prestamos.Add(pagosPrestamos);
            await _context.SaveChangesAsync();

            // Actualizar el saldo restante del préstamo
            var prestamo = await _context.gep_prestamo.FindAsync(pagosPrestamos.pag_id_prestamo);
            if (prestamo != null)
            {
                // Restar solo el monto de la cuota del saldo restante
                prestamo.pre_saldo_restante -= pagosPrestamos.pag_monto_cuota; // Asegúrate de que pag_monto_cuota no incluye la mora
                _context.Entry(prestamo).State = EntityState.Modified;

                // Actualizar la fecha de pago del préstamo al siguiente mes
                if (prestamo.pre_fecha_pago_cuotas.HasValue) // Verificar que la fecha no sea nula
                {
                    prestamo.pre_fecha_pago_cuotas = prestamo.pre_fecha_pago_cuotas.Value.AddMonths(1); // Usar .Value para acceder al DateTime
                }
                await _context.SaveChangesAsync(); // Guardar los cambios en el préstamo
            }

            return CreatedAtAction(nameof(GetPagosPrestamos), new { id = pagosPrestamos.id_pagos }, new { Message = "Pago registrado exitosamente" });
        }


        // PUT: api/pagosprestamos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPagosPrestamos(int id, PagosPrestamos pagosPrestamos)
        {
            if (id != pagosPrestamos.id_pagos)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            pagosPrestamos.pag_fecha_edicion = DateTime.UtcNow;
            _context.Entry(pagosPrestamos).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PagosPrestamosExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(new { Message = "Pago actualizado exitosamente" });
        }

        // DELETE: api/pagosprestamos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePagosPrestamos(int id)
        {
            var pagosPrestamos = await _context.gep_pagos_prestamos.FindAsync(id);
            if (pagosPrestamos == null)
            {
                return NotFound();
            }

            pagosPrestamos.pag_estado = false;
            pagosPrestamos.pag_fecha_eliminacion = DateTime.UtcNow;
            _context.Entry(pagosPrestamos).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return Ok(new { Message = "Pago eliminado exitosamente" });
        }

        private bool PagosPrestamosExists(int id)
        {
            return _context.gep_pagos_prestamos.Any(e => e.id_pagos == id);
        }
    }
}
