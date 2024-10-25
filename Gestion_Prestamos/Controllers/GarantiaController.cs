using Gestion_Prestamos.Data;
using Gestion_Prestamos.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class GarantiaController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public GarantiaController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Garantia>>> GetGarantias()
    {
        return await _context.gep_garantia.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Garantia>> GetGarantia(int id)
    {
        var garantia = await _context.gep_garantia.FindAsync(id);

        if (garantia == null)
        {
            return NotFound();
        }

        return garantia;
    }

    [HttpPost]
    public async Task<ActionResult<Garantia>> PostGarantia(Garantia garantia)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            return BadRequest(new { Message = "Model validation failed", Errors = errors });
        }

        using (var transaction = await _context.Database.BeginTransactionAsync())
        {
            try
            {
                _context.gep_garantia.Add(garantia);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return CreatedAtAction(nameof(GetGarantia), new { id = garantia.id_garantia }, garantia);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { Message = "Ocurrió un error al guardar la garantía: " + ex.Message });
            }
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutGarantia(int id, Garantia garantia)
    {
        if (id != garantia.id_garantia)
        {
            return BadRequest(new { Message = "ID de la garantía no coincide" });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        using (var transaction = await _context.Database.BeginTransactionAsync())
        {
            try
            {
                garantia.gar_fecha_edicion = DateTime.UtcNow;
                _context.Entry(garantia).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return Ok(new { Message = "Garantía actualizada exitosamente" });
            }
            catch (DbUpdateConcurrencyException)
            {
                await transaction.RollbackAsync();

                if (!GarantiaExists(id))
                {
                    return NotFound(new { Message = "Garantía no encontrada" });
                }
                else
                {
                    return StatusCode(500, new { Message = "Error de concurrencia al actualizar la garantía." });
                }
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { Message = "Ocurrió un error al actualizar la garantía: " + ex.Message });
            }
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGarantia(int id)
    {
        using (var transaction = await _context.Database.BeginTransactionAsync())
        {
            try
            {
                var garantia = await _context.gep_garantia.FindAsync(id);
                if (garantia == null)
                {
                    return NotFound(new { Message = "Garantía no encontrada" });
                }

                garantia.gar_estado = 0;
                garantia.gar_fecha_eliminacion = DateTime.UtcNow;
                _context.Entry(garantia).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return Ok(new { Message = "Garantía eliminada exitosamente" });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { Message = "Ocurrió un error al eliminar la garantía: " + ex.Message });
            }
        }
    }

    private bool GarantiaExists(int id)
    {
        return _context.gep_garantia.Any(e => e.id_garantia == id);
    }
}
