using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaGestionPersonal.Models
{
    public class Nomina
    {
        [Key]
        public int IdNomina { get; set; }

        [Required]
        public int IdEmpleado { get; set; }

        [ForeignKey("IdEmpleado")]
        public Empleado Empleado { get; set; }

        [Required]
        public decimal SalarioBruto { get; set; }

        [Required]
        public decimal Deducciones { get; set; }

        [Required]
        public decimal SalarioNeto { get; set; }

        [Required]
        public DateTime FechaPago { get; set; }
    }
}
