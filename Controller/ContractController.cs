using SistemaGestionPersonal.Data;
using SistemaGestionPersonal.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using MySql.Data.MySqlClient;

namespace SistemaGestionPersonal.Controller
{
    public class ContractController
    {
        private readonly MySqlConnectionProvider _connectionProvider;

        public ContractController(MySqlConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider ?? throw new ArgumentNullException(nameof(connectionProvider));
        }

        public void AddContract(int employeeId, DateTime startDate, DateTime endDate, string contractType)
        {
            if (startDate >= endDate)
                throw new ArgumentException("La fecha de inicio debe ser anterior a la fecha de fin.");

            using var connection = _connectionProvider.GetConnection();
            connection.Open();

            // Verificar si el empleado existe
            string employeeCheckQuery = "SELECT COUNT(*) FROM Empleado WHERE IdEmpleado = @IdEmpleado";
            using var checkCmd = new MySqlCommand(employeeCheckQuery, connection);
            checkCmd.Parameters.AddWithValue("@IdEmpleado", employeeId);

            var employeeExists = Convert.ToInt32(checkCmd.ExecuteScalar()) > 0;
            if (!employeeExists)
                throw new KeyNotFoundException("Empleado no encontrado.");

            // Insertar el contrato
            string insertQuery = @"
                INSERT INTO Contrato (IdEmpleado, FechaInicio, FechaFin, TipoContrato)
                VALUES (@IdEmpleado, @FechaInicio, @FechaFin, @TipoContrato)";
            using var insertCmd = new MySqlCommand(insertQuery, connection);
            insertCmd.Parameters.AddWithValue("@IdEmpleado", employeeId);
            insertCmd.Parameters.AddWithValue("@FechaInicio", startDate);
            insertCmd.Parameters.AddWithValue("@FechaFin", endDate);
            insertCmd.Parameters.AddWithValue("@TipoContrato", contractType);

            insertCmd.ExecuteNonQuery();
        }

        public List<Contrato> GetAllContracts()
        {
            var contracts = new List<Contrato>();

            using var connection = _connectionProvider.GetConnection();
            connection.Open();

            string query = "SELECT * FROM Contrato";
            using var command = new MySqlCommand(query, connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var contract = new Contrato
                {
                    IdContrato = reader.GetInt32("IdContrato"),
                    IdEmpleado = reader.GetInt32("IdEmpleado"),
                    FechaInicio = reader.GetDateTime("FechaInicio"),
                    FechaFin = reader.GetDateTime("FechaFin"),
                    TipoContrato = reader.GetString("TipoContrato")
                };
                contracts.Add(contract);
            }

            return contracts;
        }

        public Contrato GetContractById(int contractId)
        {
            using var connection = _connectionProvider.GetConnection();
            connection.Open();

            string query = "SELECT * FROM Contrato WHERE IdContrato = @IdContrato";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@IdContrato", contractId);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new Contrato
                {
                    IdContrato = reader.GetInt32("IdContrato"),
                    IdEmpleado = reader.GetInt32("IdEmpleado"),
                    FechaInicio = reader.GetDateTime("FechaInicio"),
                    FechaFin = reader.GetDateTime("FechaFin"),
                    TipoContrato = reader.GetString("TipoContrato")
                };
            }

            throw new KeyNotFoundException("Contrato no encontrado.");
        }

        public void UpdateContract(int contractId, int employeeId, DateTime startDate, DateTime endDate, string contractType)
        {
            if (startDate >= endDate)
                throw new ArgumentException("La fecha de inicio debe ser anterior a la fecha de fin.");

            using var connection = _connectionProvider.GetConnection();
            connection.Open();

            // Verificar si el contrato existe
            string contractCheckQuery = "SELECT COUNT(*) FROM Contrato WHERE IdContrato = @IdContrato";
            using var checkCmd = new MySqlCommand(contractCheckQuery, connection);
            checkCmd.Parameters.AddWithValue("@IdContrato", contractId);

            var contractExists = Convert.ToInt32(checkCmd.ExecuteScalar()) > 0;
            if (!contractExists)
                throw new KeyNotFoundException("Contrato no encontrado.");

            // Verificar si el empleado existe
            string employeeCheckQuery = "SELECT COUNT(*) FROM Empleado WHERE IdEmpleado = @IdEmpleado";
            using var employeeCheckCmd = new MySqlCommand(employeeCheckQuery, connection);
            employeeCheckCmd.Parameters.AddWithValue("@IdEmpleado", employeeId);

            var employeeExists = Convert.ToInt32(employeeCheckCmd.ExecuteScalar()) > 0;
            if (!employeeExists)
                throw new KeyNotFoundException("Empleado no encontrado.");

            // Actualizar el contrato
            string updateQuery = @"
                UPDATE Contrato
                SET IdEmpleado = @IdEmpleado, FechaInicio = @FechaInicio, FechaFin = @FechaFin, TipoContrato = @TipoContrato
                WHERE IdContrato = @IdContrato";
            using var updateCmd = new MySqlCommand(updateQuery, connection);
            updateCmd.Parameters.AddWithValue("@IdEmpleado", employeeId);
            updateCmd.Parameters.AddWithValue("@FechaInicio", startDate);
            updateCmd.Parameters.AddWithValue("@FechaFin", endDate);
            updateCmd.Parameters.AddWithValue("@TipoContrato", contractType);
            updateCmd.Parameters.AddWithValue("@IdContrato", contractId);

            updateCmd.ExecuteNonQuery();
        }

        public void DeleteContract(int contractId)
        {
            using var connection = _connectionProvider.GetConnection();
            connection.Open();

            // Verificar si el contrato existe
            string checkQuery = "SELECT COUNT(*) FROM Contrato WHERE IdContrato = @IdContrato";
            using var checkCmd = new MySqlCommand(checkQuery, connection);
            checkCmd.Parameters.AddWithValue("@IdContrato", contractId);

            var contractExists = Convert.ToInt32(checkCmd.ExecuteScalar()) > 0;
            if (!contractExists)
                throw new KeyNotFoundException("Contrato no encontrado.");

            // Eliminar el contrato
            string deleteQuery = "DELETE FROM Contrato WHERE IdContrato = @IdContrato";
            using var deleteCmd = new MySqlCommand(deleteQuery, connection);
            deleteCmd.Parameters.AddWithValue("@IdContrato", contractId);

            deleteCmd.ExecuteNonQuery();
        }
    }
}
