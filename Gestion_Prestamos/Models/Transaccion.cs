using System;
using System.ComponentModel.DataAnnotations;

namespace Gestion_Prestamos.Models
{
    public class Transaccion
    {
        [Key]
        public int tra_no_transaccion { get; set; }

        [Required]
        [StringLength(50)]
        public string tra_neumonico { get; set; }

        public string tra_descripcion { get; set; }

        [Required]
        public int tra_vigencia { get; set; }
        public bool tra_estado { get; set; }

        public DateTime? tra_fecha_creacion { get; set; }
        public DateTime? tra_fecha_edicion { get; set; }
        public DateTime? tra_fecha_eliminacion { get; set; }
    }
}
