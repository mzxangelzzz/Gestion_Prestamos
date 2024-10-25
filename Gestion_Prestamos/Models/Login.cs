using System.ComponentModel.DataAnnotations;

namespace Gestion_Prestamos.Models
{
    public class Login
    {
        [Key]
        public int id_user { get; set; }

        [Required]
        public string login_login { get; set; }

        [Required]
        public string login_password { get; set; }

        public int login_estado { get; set; } // Asumiendo que es un entero que indica el estado
    }
}
