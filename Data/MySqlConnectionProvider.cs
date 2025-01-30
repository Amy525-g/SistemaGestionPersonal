using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaGestionPersonal.Data
{
    public class MySqlConnectionProvider
    {
        private readonly string _connectionString;

        public MySqlConnectionProvider()
        {
            // Configura tu cadena de conexión
            _connectionString = "Server=localhost;Database=sistemagestionpersonal;User ID=root;Password=Nolosebro1;SSL Mode=None;";
        }

        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(_connectionString);
        }
    }
}