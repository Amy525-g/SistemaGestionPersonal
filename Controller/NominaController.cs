using SistemaGestionPersonal.Data;
using SistemaGestionPersonal.Models;
using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace SistemaGestionPersonal.Controller
{
    public class NominaController
    {
        private readonly MySqlConnectionProvider _connectionProvider;

        public NominaController(MySqlConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider ?? throw new ArgumentNullException(nameof(connectionProvider));
        }

        // Agregar una nueva nómina
        public void AddNomina(int empleadoId, decimal salarioBruto, decimal deducciones, DateTime fechaPago)
        {
            if (salarioBruto < deducciones)
            {
                throw new ArgumentException("Las deducciones no pueden superar el salario bruto.");
            }

            using var connection = _connectionProvider.GetConnection();
            connection.Open();

            // Verificar si el empleado existe
            string employeeCheckQuery = "SELECT COUNT(*) FROM Empleado WHERE IdEmpleado = @IdEmpleado";
            using var checkCmd = new MySqlCommand(employeeCheckQuery, connection);
            checkCmd.Parameters.AddWithValue("@IdEmpleado", empleadoId);

            var employeeExists = Convert.ToInt32(checkCmd.ExecuteScalar()) > 0;
            if (!employeeExists)
            {
                throw new KeyNotFoundException("Empleado no encontrado.");
            }

            // Insertar la nómina
            string insertQuery = @"
                INSERT INTO Nomina (IdEmpleado, SalarioBruto, Deducciones, SalarioNeto, FechaPago)
                VALUES (@IdEmpleado, @SalarioBruto, @Deducciones, @SalarioNeto, @FechaPago)";
            using var insertCmd = new MySqlCommand(insertQuery, connection);
            insertCmd.Parameters.AddWithValue("@IdEmpleado", empleadoId);
            insertCmd.Parameters.AddWithValue("@SalarioBruto", salarioBruto);
            insertCmd.Parameters.AddWithValue("@Deducciones", deducciones);
            insertCmd.Parameters.AddWithValue("@SalarioNeto", salarioBruto - deducciones);
            insertCmd.Parameters.AddWithValue("@FechaPago", fechaPago);

            insertCmd.ExecuteNonQuery();
        }

        // Obtener todas las nóminas
        public List<Nomina> GetAllNominas()
        {
            var nominas = new List<Nomina>();

            using var connection = _connectionProvider.GetConnection();
            connection.Open();

            string query = "SELECT * FROM Nomina";
            using var command = new MySqlCommand(query, connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var nomina = new Nomina
                {
                    IdNomina = reader.GetInt32("IdNomina"),
                    IdEmpleado = reader.GetInt32("IdEmpleado"),
                    SalarioBruto = reader.GetDecimal("SalarioBruto"),
                    Deducciones = reader.GetDecimal("Deducciones"),
                    SalarioNeto = reader.GetDecimal("SalarioNeto"),
                    FechaPago = reader.GetDateTime("FechaPago")
                };

                nominas.Add(nomina);
            }

            return nominas;
        }

        // Obtener una nómina por ID
        public Nomina GetNominaById(int nominaId)
        {
            using var connection = _connectionProvider.GetConnection();
            connection.Open();

            string query = "SELECT * FROM Nomina WHERE IdNomina = @IdNomina";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@IdNomina", nominaId);

            using var reader = command.ExecuteReader();

            if (reader.Read())
            {
                return new Nomina
                {
                    IdNomina = reader.GetInt32("IdNomina"),
                    IdEmpleado = reader.GetInt32("IdEmpleado"),
                    SalarioBruto = reader.GetDecimal("SalarioBruto"),
                    Deducciones = reader.GetDecimal("Deducciones"),
                    SalarioNeto = reader.GetDecimal("SalarioNeto"),
                    FechaPago = reader.GetDateTime("FechaPago")
                };
            }

            throw new KeyNotFoundException("Nómina no encontrada.");
        }

        // Actualizar una nómina existente
        public void UpdateNomina(int nominaId, decimal salarioBruto, decimal deducciones, DateTime fechaPago)
        {
            if (salarioBruto < deducciones)
            {
                throw new ArgumentException("Las deducciones no pueden superar el salario bruto.");
            }

            using var connection = _connectionProvider.GetConnection();
            connection.Open();

            // Verificar si la nómina existe
            string checkQuery = "SELECT COUNT(*) FROM Nomina WHERE IdNomina = @IdNomina";
            using var checkCmd = new MySqlCommand(checkQuery, connection);
            checkCmd.Parameters.AddWithValue("@IdNomina", nominaId);

            var nominaExists = Convert.ToInt32(checkCmd.ExecuteScalar()) > 0;
            if (!nominaExists)
            {
                throw new KeyNotFoundException("Nómina no encontrada.");
            }

            // Actualizar la nómina
            string updateQuery = @"
                UPDATE Nomina
                SET SalarioBruto = @SalarioBruto, Deducciones = @Deducciones, 
                    SalarioNeto = @SalarioNeto, FechaPago = @FechaPago
                WHERE IdNomina = @IdNomina";
            using var updateCmd = new MySqlCommand(updateQuery, connection);
            updateCmd.Parameters.AddWithValue("@SalarioBruto", salarioBruto);
            updateCmd.Parameters.AddWithValue("@Deducciones", deducciones);
            updateCmd.Parameters.AddWithValue("@SalarioNeto", salarioBruto - deducciones);
            updateCmd.Parameters.AddWithValue("@FechaPago", fechaPago);
            updateCmd.Parameters.AddWithValue("@IdNomina", nominaId);

            updateCmd.ExecuteNonQuery();
        }

        // Eliminar una nómina
        public void DeleteNomina(int nominaId)
        {
            using var connection = _connectionProvider.GetConnection();
            connection.Open();

            // Verificar si la nómina existe
            string checkQuery = "SELECT COUNT(*) FROM Nomina WHERE IdNomina = @IdNomina";
            using var checkCmd = new MySqlCommand(checkQuery, connection);
            checkCmd.Parameters.AddWithValue("@IdNomina", nominaId);

            var nominaExists = Convert.ToInt32(checkCmd.ExecuteScalar()) > 0;
            if (!nominaExists)
            {
                throw new KeyNotFoundException("Nómina no encontrada.");
            }

            // Eliminar la nómina
            string deleteQuery = "DELETE FROM Nomina WHERE IdNomina = @IdNomina";
            using var deleteCmd = new MySqlCommand(deleteQuery, connection);
            deleteCmd.Parameters.AddWithValue("@IdNomina", nominaId);

            deleteCmd.ExecuteNonQuery();
        }
    }
}
