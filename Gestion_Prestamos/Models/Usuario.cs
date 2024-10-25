using System.ComponentModel.DataAnnotations;

namespace Gestion_Prestamos.Models
{
    public class Usuario
    {
        [Key]
        public int id_user { get; set; }

        [Required]
        public int usr_id_rol { get; set; }

        [Required]
        public string usr_nombre_usuario { get; set; }

        [Required]
        [EmailAddress]
        public string usr_email { get; set; }

        [Required]
        public string usr_login { get; set; }

        [Required]
        public string usr_password { get; set; }

        public bool usr_estado { get; set; }
        public DateTime? usr_fecha_creacion { get; set; }
        public DateTime? usr_fecha_edicion { get; set; }
        public DateTime? usr_fecha_eliminacion { get; set; }
    }
}
