using SistemaGestionPersonal.Data;
using System.Collections.Generic;
using System.Linq;

namespace SistemaGestionPersonal.Controller
{
    public class ReportController
    {
        private readonly InMemoryRepository _repository;

        public ReportController(InMemoryRepository repository)
        {
            _repository = repository;
        }

        // Generar reporte de evaluaciones de desempeño
        public string GeneratePerformanceEvaluationReport()
        {
            var evaluations = _repository.EvaluacionesDesempeno
                .Select(e => $"Empleado: {e.Empleado.Nombre} {e.Empleado.Apellido}, Puntuación: {e.Puntuacion}, Fecha: {e.FechaEvaluacion.ToShortDateString()}, Comentarios: {e.Comentarios}")
                .ToList();

            return evaluations.Any()
                ? string.Join("\n", evaluations)
                : "No hay evaluaciones de desempeño disponibles.";
        }

        // Generar reporte de nóminas
        public string GeneratePayrollReport()
        {
            var payrolls = _repository.Nominas
                .Select(n => $"Empleado: {n.Empleado.Nombre} {n.Empleado.Apellido}, Salario Neto: {n.SalarioNeto}, Fecha de Pago: {n.FechaPago.ToShortDateString()}")
                .ToList();

            return payrolls.Any()
                ? string.Join("\n", payrolls)
                : "No hay nóminas disponibles.";
        }

        // Generar reporte de contratos
        public string GenerateContractsReport()
        {
            var contracts = _repository.Contratos
                .Select(c => $"Empleado: {c.Empleado.Nombre} {c.Empleado.Apellido}, Tipo: {c.TipoContrato}, Desde: {c.FechaInicio.ToShortDateString()} Hasta: {c.FechaFin.ToShortDateString()}")
                .ToList();

            return contracts.Any()
                ? string.Join("\n", contracts)
                : "No hay contratos disponibles.";
        }
    }
}