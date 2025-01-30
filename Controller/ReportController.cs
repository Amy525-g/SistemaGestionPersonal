using SistemaGestionPersonal.Data;
using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace SistemaGestionPersonal.Controller
{
    public class ReportController
    {
        private readonly MySqlConnectionProvider _connectionProvider;

        public ReportController(MySqlConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider ?? throw new ArgumentNullException(nameof(connectionProvider));
        }

        // Generar reporte de evaluaciones de desempeño
        public string GeneratePerformanceEvaluationReport()
        {
            var evaluations = new List<string>();

            using var connection = _connectionProvider.GetConnection();
            connection.Open();

            string query = @"
                SELECT e.Nombre, e.Apellido, ed.Puntuacion, ed.FechaEvaluacion, ed.Comentarios
                FROM EvaluacionDesempeno ed
                JOIN Empleado e ON ed.IdEmpleado = e.IdEmpleado";

            using var command = new MySqlCommand(query, connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                string nombre = reader.GetString("Nombre");
                string apellido = reader.GetString("Apellido");
                int puntuacion = reader.GetInt32("Puntuacion");
                DateTime fechaEvaluacion = reader.GetDateTime("FechaEvaluacion");
                string comentarios = reader.IsDBNull(reader.GetOrdinal("Comentarios")) ? "Sin comentarios" : reader.GetString("Comentarios");

                evaluations.Add($"Empleado: {nombre} {apellido}, Puntuación: {puntuacion}, Fecha: {fechaEvaluacion.ToShortDateString()}, Comentarios: {comentarios}");
            }

            return evaluations.Count > 0
                ? string.Join("\n", evaluations)
                : "No hay evaluaciones de desempeño disponibles.";
        }

        // Generar reporte de nóminas
        public string GeneratePayrollReport()
        {
            var payrolls = new List<string>();

            using var connection = _connectionProvider.GetConnection();
            connection.Open();

            string query = @"
                SELECT e.Nombre, e.Apellido, n.SalarioNeto, n.FechaPago
                FROM Nomina n
                JOIN Empleado e ON n.IdEmpleado = e.IdEmpleado";

            using var command = new MySqlCommand(query, connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                string nombre = reader.GetString("Nombre");
                string apellido = reader.GetString("Apellido");
                decimal salarioNeto = reader.GetDecimal("SalarioNeto");
                DateTime fechaPago = reader.GetDateTime("FechaPago");

                payrolls.Add($"Empleado: {nombre} {apellido}, Salario Neto: {salarioNeto}, Fecha de Pago: {fechaPago.ToShortDateString()}");
            }

            return payrolls.Count > 0
                ? string.Join("\n", payrolls)
                : "No hay nóminas disponibles.";
        }

        // Generar reporte de contratos
        public string GenerateContractsReport()
        {
            var contracts = new List<string>();

            using var connection = _connectionProvider.GetConnection();
            connection.Open();

            string query = @"
                SELECT e.Nombre, e.Apellido, c.TipoContrato, c.FechaInicio, c.FechaFin
                FROM Contrato c
                JOIN Empleado e ON c.IdEmpleado = e.IdEmpleado";

            using var command = new MySqlCommand(query, connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                string nombre = reader.GetString("Nombre");
                string apellido = reader.GetString("Apellido");
                string tipoContrato = reader.GetString("TipoContrato");
                DateTime fechaInicio = reader.GetDateTime("FechaInicio");
                DateTime fechaFin = reader.GetDateTime("FechaFin");

                contracts.Add($"Empleado: {nombre} {apellido}, Tipo: {tipoContrato}, Desde: {fechaInicio.ToShortDateString()} Hasta: {fechaFin.ToShortDateString()}");
            }

            return contracts.Count > 0
                ? string.Join("\n", contracts)
                : "No hay contratos disponibles.";
        }
    }
}
