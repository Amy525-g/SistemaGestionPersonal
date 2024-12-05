using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaGestionPersonal.Models
{
    public class Empleado
    {
        [Key]
        public int IdEmpleado { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Required]
        public string Apellido { get; set; }

        [Required]
        public DateTime FechaNacimiento { get; set; }

        public string Direccion { get; set; }

        [Required]
        public string Correo { get; set; }

        // Relación con Usuario
        public int? UserID { get; set; }
        [ForeignKey("UserID")]
        public User Usuario { get; set; }

        // Relación con Contratos
        public ICollection<Contrato> Contratos { get; set; }

        // Relación con Nóminas
        public ICollection<Nomina> Nominas { get; set; }

        // Relación con Evaluaciones de Desempeño
        public ICollection<EvaluacionDesempeno> EvaluacionesDesempeno { get; set; }
    }
}
