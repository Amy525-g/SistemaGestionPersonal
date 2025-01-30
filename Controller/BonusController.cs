using SistemaGestionPersonal.Data;
using SistemaGestionPersonal.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using MySql.Data.MySqlClient;

namespace SistemaGestionPersonal.Controller
{
    public class BonusController
    {
        private readonly MySqlConnectionProvider _connectionProvider;

        public BonusController(MySqlConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        private Dictionary<int, int> GetLatestEvaluationScores()
        {
            var evaluationScores = new Dictionary<int, int>();

            using var connection = _connectionProvider.GetConnection();
            connection.Open();

            string query = @"
                SELECT IdEmpleado, Puntuacion
                FROM EvaluacionDesempeno ed
                WHERE FechaEvaluacion = (
                    SELECT MAX(FechaEvaluacion)
                    FROM EvaluacionDesempeno
                    WHERE IdEmpleado = ed.IdEmpleado
                )";

            using var command = new MySqlCommand(query, connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var idEmpleado = reader.GetInt32("IdEmpleado");
                var puntuacion = reader.GetInt32("Puntuacion");
                evaluationScores[idEmpleado] = puntuacion;
            }

            return evaluationScores;
        }


        private (string categoria, decimal porcentaje) GetBonusCategoryAndPercentage(int puntuacion)
        {
            if (puntuacion >= 9)
                return ("Super", 30);
            if (puntuacion >= 8)
                return ("Sobresaliente", 20);
            if (puntuacion >= 6)
                return ("Bueno", 10);

            return ("No aplica", 0);
        }

        // Calcular y asignar bonos basados en las evaluaciones y contratos
        public void CalculateAndAssignBonuses()
        {
            var evaluationScores = GetLatestEvaluationScores();

            using var connection = _connectionProvider.GetConnection();
            connection.Open();

            foreach (var empleadoId in evaluationScores.Keys)
            {
                var puntuacion = evaluationScores[empleadoId];

                // Obtener contrato del empleado
                string contratoQuery = "SELECT IdEmpleado FROM Contrato WHERE IdEmpleado = @IdEmpleado";
                using var contratoCmd = new MySqlCommand(contratoQuery, connection);
                contratoCmd.Parameters.AddWithValue("@IdEmpleado", empleadoId);

                var contratoResult = contratoCmd.ExecuteScalar();
                if (contratoResult == null)
                {
                    continue; // No tiene contrato asociado
                }

                // Obtener nómina del empleado
                string nominaQuery = "SELECT SalarioNeto FROM Nomina WHERE IdEmpleado = @IdEmpleado";
                using var nominaCmd = new MySqlCommand(nominaQuery, connection);
                nominaCmd.Parameters.AddWithValue("@IdEmpleado", empleadoId);

                var salarioNetoResult = nominaCmd.ExecuteScalar();
                if (salarioNetoResult == null || Convert.ToDecimal(salarioNetoResult) <= 0)
                {
                    continue; // No tiene nómina asociada o salario es inválido
                }

                var salarioNeto = Convert.ToDecimal(salarioNetoResult);

                var (categoria, porcentaje) = GetBonusCategoryAndPercentage(puntuacion);
                if (porcentaje == 0)
                {
                    continue; // No aplica para bono
                }

                var montoTotal = salarioNeto * porcentaje / 100;

                // Insertar bono en la base de datos
                string insertBonoQuery = @"
                    INSERT INTO Bono (IdEmpleado, Categoria, Porcentaje, FechaAsignacion, MontoTotal)
                    VALUES (@IdEmpleado, @Categoria, @Porcentaje, @FechaAsignacion, @MontoTotal)";

                using var insertCmd = new MySqlCommand(insertBonoQuery, connection);
                insertCmd.Parameters.AddWithValue("@IdEmpleado", empleadoId);
                insertCmd.Parameters.AddWithValue("@Categoria", categoria);
                insertCmd.Parameters.AddWithValue("@Porcentaje", porcentaje);
                insertCmd.Parameters.AddWithValue("@FechaAsignacion", DateTime.Now);
                insertCmd.Parameters.AddWithValue("@MontoTotal", montoTotal);

                insertCmd.ExecuteNonQuery();
            }
        }

        public List<Bono> GetBonuses()
        {
            var bonuses = new List<Bono>();

            using var connection = _connectionProvider.GetConnection();
            connection.Open();

            string query = @"
        SELECT b.*, e.Nombre, e.Apellido
        FROM Bono b
        JOIN Empleado e ON b.IdEmpleado = e.IdEmpleado";

            using var command = new MySqlCommand(query, connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var bono = new Bono
                {
                    IdBono = reader.GetInt32("IdBono"),
                    IdEmpleado = reader.GetInt32("IdEmpleado"),
                    Categoria = reader.GetString("Categoria"),
                    Porcentaje = reader.GetDecimal("Porcentaje"),
                    FechaAsignacion = reader.GetDateTime("FechaAsignacion"),
                    MontoTotal = reader.GetDecimal("MontoTotal"),
                    Empleado = new Empleado
                    {
                        IdEmpleado = reader.GetInt32("IdEmpleado"),
                        Nombre = reader.GetString("Nombre"),
                        Apellido = reader.GetString("Apellido")
                    }
                };

                bonuses.Add(bono);
            }

            return bonuses;
        }

    }
}
