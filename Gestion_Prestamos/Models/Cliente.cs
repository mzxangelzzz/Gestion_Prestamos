using System.ComponentModel.DataAnnotations;

public class Cliente
{
    [Key]
    public int id_cliente { get; set; }

    [Required]
    public int cli_id_user { get; set; } = 1;

    [Required]
    [StringLength(13)]
    public string cli_DPI { get; set; }

    [Required]
    [StringLength(100)]
    public string cli_nombre { get; set; }

    [Required]
    [StringLength(100)]
    public string cli_apellido { get; set; }

    [Required]
    [EmailAddress]
    [StringLength(255)]
    public string cli_email { get; set; }

    [Required]
    public DateTime? cli_fecha_nacimiento { get; set; }

    [Required]
    [StringLength(20)]
    public string cli_telefono { get; set; }

    [Required]
    [StringLength(5)]
    public string cli_codigo_pais { get; set; }

    [Required]
    [StringLength(255)]
    public string cli_direccion { get; set; }

    public bool cli_estado { get; set; }

    public DateTime? cli_fecha_creacion { get; set; }
    public DateTime? cli_fecha_edicion { get; set; }
    public DateTime? cli_fecha_eliminacion { get; set; }
}
