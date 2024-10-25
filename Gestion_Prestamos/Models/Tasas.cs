using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gestion_Prestamos.Models
{
    public class Tasa
    {
        [Key]
        public int id_tasas { get; set; }

        [Required]
        public string tasa_descripcion { get; set; }

        [Required]
        public double tasa_porcentaje { get; set; }

        [Required]
        public DateTime? tasa_vigencia { get; set; }

        public bool tasa_estado { get; set; }

        public DateTime? tasa_fecha_creacion { get; set; }
        public DateTime? tasa_fecha_edicion { get; set; }
        public DateTime? tasa_fecha_eliminacion { get; set; }
    }
}