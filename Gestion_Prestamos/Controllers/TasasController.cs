using Gestion_Prestamos.Data;
using Gestion_Prestamos.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class TasaController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public TasaController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Tasa>>> GetTasa()
    {
        return await _context.gep_tasas.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Tasa>> GetTasa(int id)
    {
        var tasa = await _context.gep_tasas.FindAsync(id);

        if (tasa == null)
        {
            return NotFound();
        }

        return tasa;
    }

    [HttpPost]
    public async Task<ActionResult<Tasa>> PostCliente(Tasa tasa)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (await _context.gep_tasas.AnyAsync(c => c.tasa_descripcion == tasa.tasa_descripcion))
        {
            return Conflict(new { Message = "La Descripcion de Tasa ya esta en uso." });
        }

        using (var transaction = await _context.Database.BeginTransactionAsync())
        {
            try
            {
                tasa.tasa_estado = true;
                tasa.tasa_fecha_creacion = DateTime.UtcNow;

                _context.gep_tasas.Add(tasa);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return CreatedAtAction(nameof(GetTasa), new { id = tasa.id_tasas }, tasa);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { Message = "Ocurrió un error al guardar la tasa: " + ex.Message });
            }
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutTasa(int id, Tasa tasa)
    {
        if (id != tasa.id_tasas)
        {
            return BadRequest(new { Message = "ID de la tasa no coincide" });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (await _context.gep_tasas.AnyAsync(c => c.tasa_descripcion == tasa.tasa_descripcion && c.id_tasas != id))
        {
            return Conflict(new { Message = "La Descripcion de la tasa ya está en uso." });
        }

        using (var transaction = await _context.Database.BeginTransactionAsync())
        {
            try
            {
                tasa.tasa_fecha_edicion = DateTime.UtcNow;
                _context.Entry(tasa).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return Ok(new { Message = "Tasa actualizada exitosamente" });
            }
            catch (DbUpdateConcurrencyException)
            {
                await transaction.RollbackAsync();

                if (!TasaExists(id))
                {
                    return NotFound(new { Message = "Tasa no encontrada" });
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { Message = "Ocurrió un error al actualizar la tasa: " + ex.Message });
            }
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTasa(int id)
    {
        using (var transaction = await _context.Database.BeginTransactionAsync())
        {
            try
            {
                var tasa = await _context.gep_tasas.FindAsync(id);
                if (tasa == null)
                {
                    return NotFound(new { Message = "Tasa no encontrado" });
                }

                tasa.tasa_estado = false;
                tasa.tasa_fecha_eliminacion = DateTime.UtcNow;
                _context.Entry(tasa).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return Ok(new { Message = "Tasa eliminada exitosamente" });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { Message = "Ocurrió un error al eliminar la tasa: " + ex.Message });
            }
        }
    }

    private bool TasaExists(int id)
    {
        return _context.gep_tasas.Any(e => e.id_tasas == id);
    }
}

