using MySql.Data.MySqlClient;
using SistemaGestionPersonal.Data;
using SistemaGestionPersonal.Models;

namespace SistemaGestionPersonal.Controller;

public class LoginController
{
    private readonly MySqlConnectionProvider _connectionProvider;

    public LoginController(MySqlConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider ?? throw new ArgumentNullException(nameof(connectionProvider));
    }

    public User AuthenticateUser(string username, string password)
    {
        using var connection = _connectionProvider.GetConnection();
        connection.Open();

        string query = @"
            SELECT u.UserID, u.Username, u.PasswordHash, r.RoleID, r.RoleName
            FROM User u
            JOIN Role r ON u.RoleID = r.RoleID
            WHERE u.Username = @Username";

        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@Username", username);

        using var reader = command.ExecuteReader();

        if (reader.Read())
        {
            string storedPasswordHash = reader.GetString("PasswordHash");

            // Verificar la contraseña (implementa un hash seguro como BCrypt o SHA-256)
            if (storedPasswordHash != password) // Simula la comparación, usa hash real
                return null;

            return new User
            {
                UserID = reader.GetInt32("UserID"),
                Username = reader.GetString("Username"),
                PasswordHash = storedPasswordHash,
                Role = new Role
                {
                    RoleID = reader.GetInt32("RoleID"),
                    RoleName = reader.GetString("RoleName")
                }
            };
        }

        return null;
    }
}
