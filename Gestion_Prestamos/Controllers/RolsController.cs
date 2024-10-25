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
    public class RolesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RolesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/roles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Rol>>> GetRoles()
        {
            return await _context.gep_rol.ToListAsync();
        }

        // GET: api/roles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Rol>> GetRol(int id)
        {
            var rol = await _context.gep_rol.FindAsync(id);

            if (rol == null)
            {
                return NotFound();
            }

            return rol;
        }

        [HttpPost]
        public async Task<ActionResult<Rol>> PostRol(Rol rol)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    rol.rol_estado = true;
                    rol.rol_fecha_creacion = DateTime.UtcNow;

                    _context.gep_rol.Add(rol);
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();

                    return CreatedAtAction(nameof(GetRol), new { id = rol.id_rol }, rol);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(500, "Ocurrió un error al guardar el rol: " + ex.Message);
                }
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutRol(int id, Rol rol)
        {
            if (id != rol.id_rol)
            {
                return BadRequest();
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Actualizar fecha de edición
                    rol.rol_fecha_edicion = DateTime.UtcNow;
                    _context.Entry(rol).State = EntityState.Modified;

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return NoContent();
                }
                catch (DbUpdateConcurrencyException)
                {
                    await transaction.RollbackAsync();

                    if (!RolExists(id))
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
                    return StatusCode(500, "Ocurrió un error al actualizar el rol: " + ex.Message);
                }
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRol(int id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var rol = await _context.gep_rol.FindAsync(id);
                    if (rol == null)
                    {
                        return NotFound();
                    }

                    // Cambiar estado y actualizar fecha de eliminación
                    rol.rol_estado = false;
                    rol.rol_fecha_eliminacion = DateTime.UtcNow;
                    _context.Entry(rol).State = EntityState.Modified;
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();

                    return NoContent();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(500, "Ocurrió un error al eliminar el rol: " + ex.Message);
                }
            }
        }

        private bool RolExists(int id)
        {
            return _context.gep_rol.Any(e => e.id_rol == id);
        }
    }
}
