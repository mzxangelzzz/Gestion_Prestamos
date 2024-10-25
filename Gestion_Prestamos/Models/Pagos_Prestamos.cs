using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class PagosPrestamos
{
    [Key]
    public int id_pagos { get; set; }

    [Required]
    public int pag_id_prestamo { get; set; }

    [Required]
    [Column(TypeName = "decimal(10, 2)")]
    public decimal pag_monto_cuota { get; set; }

    [Required]
    [Column(TypeName = "decimal(10, 2)")]
    public decimal pag_mora {  get; set; }

    [Required]
    [Column(TypeName = "decimal(10, 2)")]
    public decimal pag_total_pagado { get; set; }

    [Required]
    public DateTime pag_fecha_pago { get; set; }

    [Required]
    [StringLength(50)]
    public string pag_metodo_pago { get; set; }

    public bool? pag_pago_extraordinario { get; set; }

    public bool pag_estado { get; set; }

    public DateTime? pag_fecha_creacion { get; set; }
    public DateTime? pag_fecha_edicion { get; set; }
    public DateTime? pag_fecha_eliminacion { get; set; }
}
