using SistemaGestionPersonal.Data;
using SistemaGestionPersonal.Models;
using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace SistemaGestionPersonal.Controller
{
    public class EvaluationController
    {
        private readonly MySqlConnectionProvider _connectionProvider;

        public EvaluationController(MySqlConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider ?? throw new ArgumentNullException(nameof(connectionProvider));
        }

        // Agregar una nueva evaluación de desempeño
        public void AddEvaluation(int empleadoId, DateTime fechaEvaluacion, int puntuacion, string comentarios)
        {
            if (puntuacion < 1 || puntuacion > 10)
                throw new Exception("La puntuación debe estar entre 1 y 10.");

            using var connection = _connectionProvider.GetConnection();
            connection.Open();

            // Verificar si el empleado existe
            string employeeCheckQuery = "SELECT COUNT(*) FROM Empleado WHERE IdEmpleado = @IdEmpleado";
            using var checkCmd = new MySqlCommand(employeeCheckQuery, connection);
            checkCmd.Parameters.AddWithValue("@IdEmpleado", empleadoId);

            var employeeExists = Convert.ToInt32(checkCmd.ExecuteScalar()) > 0;
            if (!employeeExists)
                throw new Exception("Empleado no encontrado.");

            // Insertar la evaluación
            string insertQuery = @"
                INSERT INTO EvaluacionDesempeno (IdEmpleado, FechaEvaluacion, Puntuacion, Comentarios)
                VALUES (@IdEmpleado, @FechaEvaluacion, @Puntuacion, @Comentarios)";
            using var insertCmd = new MySqlCommand(insertQuery, connection);
            insertCmd.Parameters.AddWithValue("@IdEmpleado", empleadoId);
            insertCmd.Parameters.AddWithValue("@FechaEvaluacion", fechaEvaluacion);
            insertCmd.Parameters.AddWithValue("@Puntuacion", puntuacion);
            insertCmd.Parameters.AddWithValue("@Comentarios", comentarios);

            insertCmd.ExecuteNonQuery();
        }

        // Obtener todas las evaluaciones de desempeño
        public List<EvaluacionDesempeno> GetAllEvaluations()
        {
            var evaluations = new List<EvaluacionDesempeno>();

            using var connection = _connectionProvider.GetConnection();
            connection.Open();

            string query = "SELECT * FROM EvaluacionDesempeno";
            using var command = new MySqlCommand(query, connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var evaluacion = new EvaluacionDesempeno
                {
                    IdEvaluacion = reader.GetInt32("IdEvaluacion"),
                    IdEmpleado = reader.GetInt32("IdEmpleado"),
                    FechaEvaluacion = reader.GetDateTime("FechaEvaluacion"),
                    Puntuacion = reader.GetInt32("Puntuacion"),
                    Comentarios = reader.IsDBNull(reader.GetOrdinal("Comentarios")) ? null : reader.GetString("Comentarios")
                };

                evaluations.Add(evaluacion);
            }

            return evaluations;
        }

        // Actualizar una evaluación de desempeño
        public void UpdateEvaluation(int idEvaluacion, DateTime fechaEvaluacion, int puntuacion, string comentarios)
        {
            if (puntuacion < 1 || puntuacion > 10)
                throw new Exception("La puntuación debe estar entre 1 y 10.");

            using var connection = _connectionProvider.GetConnection();
            connection.Open();

            // Verificar si la evaluación existe
            string checkQuery = "SELECT COUNT(*) FROM EvaluacionDesempeno WHERE IdEvaluacion = @IdEvaluacion";
            using var checkCmd = new MySqlCommand(checkQuery, connection);
            checkCmd.Parameters.AddWithValue("@IdEvaluacion", idEvaluacion);

            var evaluationExists = Convert.ToInt32(checkCmd.ExecuteScalar()) > 0;
            if (!evaluationExists)
                throw new Exception("Evaluación no encontrada.");

            // Actualizar la evaluación
            string updateQuery = @"
                UPDATE EvaluacionDesempeno
                SET FechaEvaluacion = @FechaEvaluacion, Puntuacion = @Puntuacion, Comentarios = @Comentarios
                WHERE IdEvaluacion = @IdEvaluacion";
            using var updateCmd = new MySqlCommand(updateQuery, connection);
            updateCmd.Parameters.AddWithValue("@IdEvaluacion", idEvaluacion);
            updateCmd.Parameters.AddWithValue("@FechaEvaluacion", fechaEvaluacion);
            updateCmd.Parameters.AddWithValue("@Puntuacion", puntuacion);
            updateCmd.Parameters.AddWithValue("@Comentarios", comentarios);

            updateCmd.ExecuteNonQuery();
        }

        // Eliminar una evaluación de desempeño
        public void DeleteEvaluation(int idEvaluacion)
        {
            using var connection = _connectionProvider.GetConnection();
            connection.Open();

            // Verificar si la evaluación existe
            string checkQuery = "SELECT COUNT(*) FROM EvaluacionDesempeno WHERE IdEvaluacion = @IdEvaluacion";
            using var checkCmd = new MySqlCommand(checkQuery, connection);
            checkCmd.Parameters.AddWithValue("@IdEvaluacion", idEvaluacion);

            var evaluationExists = Convert.ToInt32(checkCmd.ExecuteScalar()) > 0;
            if (!evaluationExists)
                throw new Exception("Evaluación no encontrada.");

            // Eliminar la evaluación
            string deleteQuery = "DELETE FROM EvaluacionDesempeno WHERE IdEvaluacion = @IdEvaluacion";
            using var deleteCmd = new MySqlCommand(deleteQuery, connection);
            deleteCmd.Parameters.AddWithValue("@IdEvaluacion", idEvaluacion);

            deleteCmd.ExecuteNonQuery();
        }
    }
}
