using System.ComponentModel.DataAnnotations;

namespace Gestion_Prestamos.Models
{
    public class Garantia
    {
        [Key]
        public int id_garantia { get; set; }

        [Required]
        public int id_cliente { get; set; }

        [Required]
        public string gar_tipo_garantia { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "El valor debe ser mayor a 0.")]
        public decimal gar_valor { get; set; }

        [Required]
        public string gar_descripcion { get; set; }

        public int gar_estado { get; set; } = 1;

        public DateTime gar_fecha_creacion { get; set; } = DateTime.UtcNow;

        public DateTime? gar_fecha_edicion { get; set; } = null;

        public DateTime? gar_fecha_eliminacion { get; set; } = null;
    }
}
