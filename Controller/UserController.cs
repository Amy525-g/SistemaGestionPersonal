using SistemaGestionPersonal.Data;
using SistemaGestionPersonal.Models;
using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace SistemaGestionPersonal.Controller
{
    public class UserController
    {
        private readonly MySqlConnectionProvider _connectionProvider;

        public UserController(MySqlConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider ?? throw new ArgumentNullException(nameof(connectionProvider));
        }

        // Método para obtener todos los roles
        public List<Role> GetRoles()
        {
            var roles = new List<Role>();

            using var connection = _connectionProvider.GetConnection();
            connection.Open();

            string query = "SELECT * FROM Role";
            using var command = new MySqlCommand(query, connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                roles.Add(new Role
                {
                    RoleID = reader.GetInt32("RoleID"),
                    RoleName = reader.GetString("RoleName")
                });
            }

            return roles;
        }

        // 1. Crear Usuario
        public void AddUser(string username, string password, string roleName)
        {
            using var connection = _connectionProvider.GetConnection();
            connection.Open();

            // Verifica que el rol exista
            string roleQuery = "SELECT RoleID FROM Role WHERE RoleName = @RoleName";
            using var roleCommand = new MySqlCommand(roleQuery, connection);
            roleCommand.Parameters.AddWithValue("@RoleName", roleName);

            var roleId = roleCommand.ExecuteScalar();
            if (roleId == null)
            {
                throw new Exception("El rol especificado no existe.");
            }

            // Verifica si el nombre de usuario ya existe
            string userCheckQuery = "SELECT COUNT(*) FROM User WHERE Username = @Username";
            using var userCheckCommand = new MySqlCommand(userCheckQuery, connection);
            userCheckCommand.Parameters.AddWithValue("@Username", username);

            if (Convert.ToInt32(userCheckCommand.ExecuteScalar()) > 0)
            {
                throw new Exception("El nombre de usuario ya existe.");
            }

            // Inserta un nuevo usuario
            string insertQuery = @"
                INSERT INTO User (Username, PasswordHash, RoleID)
                VALUES (@Username, @PasswordHash, @RoleID)";
            using var insertCommand = new MySqlCommand(insertQuery, connection);
            insertCommand.Parameters.AddWithValue("@Username", username);
            insertCommand.Parameters.AddWithValue("@PasswordHash", password); // Aquí deberías aplicar un hash seguro
            insertCommand.Parameters.AddWithValue("@RoleID", roleId);

            insertCommand.ExecuteNonQuery();
        }

        // 2. Leer Todos los Usuarios
        public List<User> GetAllUsers()
        {
            var users = new List<User>();

            using var connection = _connectionProvider.GetConnection();
            connection.Open();

            string query = @"
                SELECT u.UserID, u.Username, u.PasswordHash, r.RoleID, r.RoleName
                FROM User u
                JOIN Role r ON u.RoleID = r.RoleID";

            using var command = new MySqlCommand(query, connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                users.Add(new User
                {
                    UserID = reader.GetInt32("UserID"),
                    Username = reader.GetString("Username"),
                    PasswordHash = reader.GetString("PasswordHash"),
                    RoleID = reader.GetInt32("RoleID"),
                    Role = new Role
                    {
                        RoleID = reader.GetInt32("RoleID"),
                        RoleName = reader.GetString("RoleName")
                    }
                });
            }

            return users;
        }

        // 3. Leer un Usuario por ID
        public User GetUserById(int userId)
        {
            using var connection = _connectionProvider.GetConnection();
            connection.Open();

            string query = @"
                SELECT u.UserID, u.Username, u.PasswordHash, r.RoleID, r.RoleName
                FROM User u
                JOIN Role r ON u.RoleID = r.RoleID
                WHERE u.UserID = @UserID";

            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserID", userId);

            using var reader = command.ExecuteReader();

            if (reader.Read())
            {
                return new User
                {
                    UserID = reader.GetInt32("UserID"),
                    Username = reader.GetString("Username"),
                    PasswordHash = reader.GetString("PasswordHash"),
                    RoleID = reader.GetInt32("RoleID"),
                    Role = new Role
                    {
                        RoleID = reader.GetInt32("RoleID"),
                        RoleName = reader.GetString("RoleName")
                    }
                };
            }

            throw new Exception("Usuario no encontrado.");
        }

        // 4. Actualizar Usuario
        public void UpdateUser(int userId, string newUsername, string newPassword, string newRoleName)
        {
            using var connection = _connectionProvider.GetConnection();
            connection.Open();

            // Verifica que el usuario exista
            string userCheckQuery = "SELECT COUNT(*) FROM User WHERE UserID = @UserID";
            using var userCheckCommand = new MySqlCommand(userCheckQuery, connection);
            userCheckCommand.Parameters.AddWithValue("@UserID", userId);

            if (Convert.ToInt32(userCheckCommand.ExecuteScalar()) == 0)
            {
                throw new Exception("Usuario no encontrado.");
            }

            // Verifica que el rol exista
            string roleQuery = "SELECT RoleID FROM Role WHERE RoleName = @RoleName";
            using var roleCommand = new MySqlCommand(roleQuery, connection);
            roleCommand.Parameters.AddWithValue("@RoleName", newRoleName);

            var roleId = roleCommand.ExecuteScalar();
            if (roleId == null)
            {
                throw new Exception("El rol especificado no existe.");
            }

            // Actualiza los datos del usuario
            string updateQuery = @"
                UPDATE User
                SET Username = @Username, PasswordHash = @PasswordHash, RoleID = @RoleID
                WHERE UserID = @UserID";
            using var updateCommand = new MySqlCommand(updateQuery, connection);
            updateCommand.Parameters.AddWithValue("@Username", newUsername);
            updateCommand.Parameters.AddWithValue("@PasswordHash", newPassword); // Aquí también deberías aplicar un hash seguro
            updateCommand.Parameters.AddWithValue("@RoleID", roleId);
            updateCommand.Parameters.AddWithValue("@UserID", userId);

            updateCommand.ExecuteNonQuery();
        }

        // 5. Eliminar Usuario
        public void DeleteUser(int userId)
        {
            using var connection = _connectionProvider.GetConnection();
            connection.Open();

            // Verifica que el usuario exista
            string checkQuery = "SELECT COUNT(*) FROM User WHERE UserID = @UserID";
            using var checkCommand = new MySqlCommand(checkQuery, connection);
            checkCommand.Parameters.AddWithValue("@UserID", userId);

            if (Convert.ToInt32(checkCommand.ExecuteScalar()) == 0)
            {
                throw new Exception("Usuario no encontrado.");
            }

            // Elimina el usuario
            string deleteQuery = "DELETE FROM User WHERE UserID = @UserID";
            using var deleteCommand = new MySqlCommand(deleteQuery, connection);
            deleteCommand.Parameters.AddWithValue("@UserID", userId);

            deleteCommand.ExecuteNonQuery();
        }
    }
}
