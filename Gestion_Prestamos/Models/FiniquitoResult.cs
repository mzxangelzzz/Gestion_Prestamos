using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gestion_Prestamos.Models
{
    public class FiniquitoResult
    {
        public int IdPrestamo { get; set; }
        public string ClienteNombre { get; set; }
        public string ClienteApellido { get; set; }
        public decimal MontoPrestamo { get; set; }
        public decimal SaldoRestante { get; set; }  
        public decimal TotalPagado { get; set; }
        public decimal CancelacionAnticipada { get; set; }
        public decimal Penalidad { get; set; }
        public decimal Finiquito { get; set; }
    }
}
