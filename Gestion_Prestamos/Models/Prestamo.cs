using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gestion_Prestamos.Models
{
    public class Prestamo
    {
        [Key]
        public int id_prestamo { get; set; }

        [Required]
        public int pre_id_cliente { get; set; }

        [Required]
        public int pre_id_usuario { get; set; } = 1;

        [Required]
        public int? pre_id_garantia { get; set; }

        [Required]
        public int pre_id_tasas { get; set; }

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal pre_monto_prestamo { get; set; }

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal pre_saldo_restante { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal pre_monto_cuotas { get; set; }

        [Required]
        public int pre_plazo_prestamo { get; set; }

        [Required]
        [StringLength(255)]
        public string pre_tipo_prestamo { get; set; }

        [Required]
        [StringLength(255)]
        public string pre_estado_prestamo { get; set; }

        public DateTime? pre_fecha_aprobacion { get; set; }
        public DateTime? pre_fecha_pago_cuotas { get; set; }
        public bool pre_aprobacion { get; set; }
        public bool pre_estado { get; set; }
        public DateTime? pre_fecha_creacion { get; set; }
        public DateTime? pre_fecha_edicion { get; set; }
        public DateTime? pre_fecha_eliminacion { get; set; }
    }
}
