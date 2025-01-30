using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaGestionPersonal.Models
{
    public class EvaluationData
    {
        public int IdEvaluacion { get; set; }
        public DateTime FechaEvaluacion { get; set; }
        public int Puntuacion { get; set; }
        public string Comentarios { get; set; }
        public string EmpleadoNombre { get; set; }
    }
}