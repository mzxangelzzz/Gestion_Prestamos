using System;
using System.ComponentModel.DataAnnotations;

namespace Gestion_Prestamos.Models
{
    public class CancelacionAnticipadaPrestamo
    {
        [Key]
        public int cap_id_cancelacion { get; set; }

        [Required]
        public int cap_prestamo { get; set; }

        [Required]
        public DateTime cap_fecha_cancelacion { get; set; }

        [Required]
        public decimal cap_monto_cancelado { get; set; }

        [Required]
        public int cap_usuario { get; set; } = 1;

        [Required]
        public decimal cap_penalidad { get; set; }

        public string cap_comentarios { get; set; }
        public bool cap_estado { get; set; }
        public DateTime? cap_fecha_creacion { get; set; }
        /*
        public DateTime? cap_fecha_edicion { get; set; }
        public DateTime? cap_fecha_eliminacion { get; set; }
        */
    }
}
