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
    public class UsuariosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsuariosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/usuarios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
        {
            return await _context.gep_usuario.ToListAsync();
        }

        // GET: api/usuarios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetUsuario(int id)
        {
            var usuario = await _context.gep_usuario.FindAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            return usuario;
        }

        [HttpPost]
        public async Task<ActionResult<Usuario>> PostUsuario(Usuario usuario)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Verificar si el email o el login ya existen
            if (await _context.gep_usuario.AnyAsync(u => u.usr_email == usuario.usr_email || u.usr_login == usuario.usr_login))
            {
                return Conflict(new { Message = "El correo electrónico o el login ya están en uso." });
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    usuario.usr_estado = true;
                    usuario.usr_fecha_creacion = DateTime.UtcNow;

                    _context.gep_usuario.Add(usuario);
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();

                    return CreatedAtAction(nameof(GetUsuario), new { id = usuario.id_user }, new { Message = "Usuario agregado exitosamente" });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(500, "Ocurrió un error al guardar el usuario: " + ex.Message);
                }
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuario(int id, Usuario usuario)
        {
            if (id != usuario.id_user)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Verificar si el email o el login ya están en uso por otro usuario
            if (await _context.gep_usuario.AnyAsync(u => (u.usr_email == usuario.usr_email || u.usr_login == usuario.usr_login) && u.id_user != id))
            {
                return Conflict(new { Message = "El correo electrónico o el login ya están en uso." });
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    usuario.usr_fecha_edicion = DateTime.UtcNow; // Actualizar fecha de edición
                    _context.Entry(usuario).State = EntityState.Modified;
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();

                    return Ok(new { Message = "Usuario actualizado exitosamente" });
                }
                catch (DbUpdateConcurrencyException)
                {
                    await transaction.RollbackAsync();

                    if (!UsuarioExists(id))
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
                    return StatusCode(500, "Ocurrió un error al actualizar el usuario: " + ex.Message);
                }
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var usuario = await _context.gep_usuario.FindAsync(id);
                    if (usuario == null)
                    {
                        return NotFound();
                    }

                    usuario.usr_estado = false;
                    usuario.usr_fecha_eliminacion = DateTime.UtcNow; // Actualizar fecha de eliminación
                    _context.Entry(usuario).State = EntityState.Modified;
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();

                    return Ok(new { Message = "Usuario eliminado exitosamente" });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(500, "Ocurrió un error al eliminar el usuario: " + ex.Message);
                }
            }
        }

        private bool UsuarioExists(int id)
        {
            return _context.gep_usuario.Any(e => e.id_user == id);
        }
    }
}
