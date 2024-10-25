using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Gestion_Prestamos.Data;
using Gestion_Prestamos.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Gestion_Prestamos.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LoginController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: api/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Login loginRequest)
        {
            var usuario = await _context.gep_login
                .Where(u => u.login_login == loginRequest.login_login
                         && u.login_password == loginRequest.login_password
                         && u.login_estado == 1) // Verifica que el estado sea activo (1)
                .FirstOrDefaultAsync();

            if (usuario == null)
            {
                return Unauthorized(new { message = "Usuario, Contraseña o Estado Incorrectos" });
            }

            return Ok(new { message = "Login Exitoso" });
        }
    }
}
