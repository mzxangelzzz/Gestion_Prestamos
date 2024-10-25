using Gestion_Prestamos.Data;
using Gestion_Prestamos.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class ClientesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IHttpClientFactory _httpClientFactory;

    public ClientesController(ApplicationDbContext context, IHttpClientFactory httpClientFactory)
    {
        _context = context;
        _httpClientFactory = httpClientFactory;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Cliente>>> GetClientes()
    {
        return await _context.gep_clientes.ToListAsync();
    }

    [HttpGet("check-email")]
    public async Task<IActionResult> CheckEmail(string email)
    {
        var emailExists = await _context.gep_clientes.AnyAsync(c => c.cli_email == email);
        return Ok(new { EmailExists = emailExists });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Cliente>> GetCliente(int id)
    {
        var cliente = await _context.gep_clientes.FindAsync(id);

        if (cliente == null)
        {
            return NotFound();
        }

        return cliente;
    }

    [HttpPost]
    public async Task<ActionResult<Cliente>> PostCliente(Cliente cliente)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            return BadRequest(new { Message = "Model validation failed", Errors = errors });
        }

        if (await _context.gep_clientes.AnyAsync(c => c.cli_email == cliente.cli_email || c.cli_DPI == cliente.cli_DPI))
        {
            return Conflict(new { Message = "El correo electrónico o DPI ya está en uso." });
        }

        using (var transaction = await _context.Database.BeginTransactionAsync())
        {
            try
            {
                // Asignar estado activo y fecha de creación
                cliente.cli_estado = true;
                cliente.cli_fecha_creacion = DateTime.UtcNow;

                _context.gep_clientes.Add(cliente);
                await _context.SaveChangesAsync();

                // Enviar solicitud al backend de Infonet
                var httpClient = _httpClientFactory.CreateClient();
                var infonetCliente = new
                {
                    dpi = cliente.cli_DPI,
                    nombre = cliente.cli_nombre,
                    apellido = cliente.cli_apellido
                };

                var content = new StringContent(JsonSerializer.Serialize(infonetCliente), Encoding.UTF8, "application/json");

                // URL correcta del backend de Infonet
                string infonetUrl = "https://localhost:7258/api/Infonet/registrar-cliente";
                var response = await httpClient.PostAsync(infonetUrl, content);

                if (!response.IsSuccessStatusCode)
                {
                    // Capturar el contenido de la respuesta de error
                    var responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Error en la respuesta de Infonet: " + responseContent); // Log del contenido de la respuesta

                    return StatusCode((int)response.StatusCode, new
                    {
                        Message = "Error al comunicar con Infonet.",
                        Details = responseContent
                    });
                }

                // Si todo va bien, confirmar la transacción
                await transaction.CommitAsync();

                return CreatedAtAction(nameof(GetCliente), new { id = cliente.id_cliente }, cliente);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                var innerException = ex.InnerException != null ? ex.InnerException.Message : "No inner exception";
                return StatusCode(500, new { Message = "Ocurrió un error al guardar el cliente: " + ex.Message, InnerException = innerException });
            }
        }
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> PutCliente(int id, Cliente cliente)
    {
        if (id != cliente.id_cliente)
        {
            return BadRequest(new { Message = "ID del cliente no coincide" });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (await _context.gep_clientes.AnyAsync(c => c.cli_email == cliente.cli_email && c.id_cliente != id))
        {
            return Conflict(new { Message = "El correo electrónico ya está en uso por otro cliente." });
        }

        if (await _context.gep_clientes.AnyAsync(c => c.cli_DPI == cliente.cli_DPI && c.id_cliente != id))
        {
            return Conflict(new { Message = "El DPI ya está en uso por otro cliente." });
        }

        using (var transaction = await _context.Database.BeginTransactionAsync())
        {
            try
            {
                cliente.cli_fecha_edicion = DateTime.UtcNow;
                _context.Entry(cliente).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return Ok(new { Message = "Cliente actualizado exitosamente" });
            }
            catch (DbUpdateConcurrencyException)
            {
                await transaction.RollbackAsync();

                if (!ClienteExists(id))
                {
                    return NotFound(new { Message = "Cliente no encontrado" });
                }
                else
                {
                    return StatusCode(500, new { Message = "Error de concurrencia al actualizar el cliente." });
                }
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { Message = "Ocurrió un error al actualizar el cliente: " + ex.Message });
            }
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCliente(int id)
    {
        using (var transaction = await _context.Database.BeginTransactionAsync())
        {
            try
            {
                var cliente = await _context.gep_clientes.FindAsync(id);
                if (cliente == null)
                {
                    return NotFound(new { Message = "Cliente no encontrado" });
                }

                cliente.cli_estado = false;
                cliente.cli_fecha_eliminacion = DateTime.UtcNow;
                _context.Entry(cliente).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return Ok(new { Message = "Cliente eliminado exitosamente" });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { Message = "Ocurrió un error al eliminar el cliente: " + ex.Message });
            }
        }
    }

    private bool ClienteExists(int id)
    {
        return _context.gep_clientes.Any(e => e.id_cliente == id);
    }
}
