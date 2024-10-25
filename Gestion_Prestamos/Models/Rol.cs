using System.ComponentModel.DataAnnotations;

namespace Gestion_Prestamos.Models
{
    public class Rol
    {
        [Key]
        public int id_rol { get; set; }
        
        [Required]
        [MaxLength(255)]
        public string rol_descripcion { get; set; }

        [Required]
        public bool rol_estado { get; set; }

        public DateTime? rol_fecha_creacion { get; set; }
        public DateTime? rol_fecha_edicion { get; set; }
        public DateTime? rol_fecha_eliminacion { get; set; }
    }
}
