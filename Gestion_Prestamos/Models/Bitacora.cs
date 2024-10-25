using System;
using System.ComponentModel.DataAnnotations;

namespace Gestion_Prestamos.Models
{
    public class Bitacora
    {
        [Key]
        public int id_secuencial { get; set; }

        [Required]
        public int btn_id_prestamo { get; set; }

        [Required]
        public DateTime? btn_fecha { get; set; }

        [Required]
        public decimal btn_monto { get; set; }

        public bool btn_estado { get; set; }
        public DateTime? btn_fecha_creacion { get; set; }
        public DateTime? btn_fecha_edicion { get; set; }
        public DateTime? btn_fecha_eliminacion { get; set; }
    }
}
