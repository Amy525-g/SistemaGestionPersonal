using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaGestionPersonal.Models
{
    public class EvaluacionDesempeno
    {
        [Key]
        public int IdEvaluacion { get; set; }

        [Required]
        public int IdEmpleado { get; set; }

        [ForeignKey("IdEmpleado")]
        public Empleado Empleado { get; set; }

        [Required]
        public DateTime FechaEvaluacion { get; set; }

        [Required]
        [Range(1, 10)]
        public int Puntuacion { get; set; }

        public string Comentarios { get; set; }
    }
}
