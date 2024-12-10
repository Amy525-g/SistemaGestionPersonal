using System;

namespace SistemaGestionPersonal.Models
{
    public class Bono
    {
        public int IdBono { get; set; }
        public int IdEmpleado { get; set; }
        public string Categoria { get; set; }
        public decimal Porcentaje { get; set; }
        public DateTime FechaAsignacion { get; set; }
        public decimal MontoTotal { get; set; }
        public Empleado Empleado { get; set; }
    }
}
