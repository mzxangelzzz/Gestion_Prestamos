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
    public class BitacoraController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BitacoraController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/bitacora
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Bitacora>>> GetBitacora()
        {
            return await _context.gep_bitacora_de_prestamos.ToListAsync();
        }

        // GET: api/bitacora/prestamo/{id}
        [HttpGet("prestamo/{id}")]
        public async Task<ActionResult<IEnumerable<Bitacora>>> GetBitacoraByPrestamo(int id)
        {
            var bitacora = await _context.gep_bitacora_de_prestamos
                                         .Where(b => b.btn_id_prestamo == id)
                                         .ToListAsync();

            if (bitacora == null || bitacora.Count == 0)
            {
                return NotFound(new { Message = "No se encontraron registros para este préstamo" });
            }

            return bitacora;
        }

        // GET: api/bitacora/fecha
        [HttpGet("fecha")]
        public async Task<ActionResult<IEnumerable<Bitacora>>> GetBitacoraByFecha([FromQuery] DateTime fechaInicio, [FromQuery] DateTime fechaFin)
        {
            var bitacora = await _context.gep_bitacora_de_prestamos
                                         .Where(b => b.btn_fecha >= fechaInicio && b.btn_fecha <= fechaFin)
                                         .ToListAsync();

            if (bitacora == null || bitacora.Count == 0)
            {
                return NotFound(new { Message = "No se encontraron registros en el rango de fechas especificado" });
            }

            return bitacora;
        }
    }
}
