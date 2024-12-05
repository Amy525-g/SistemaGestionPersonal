using SistemaGestionPersonal.Data;
using SistemaGestionPersonal.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SistemaGestionPersonal.Controller
{
    public class UserController
    {
        private readonly InMemoryRepository _repository;

        public UserController(InMemoryRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        // Método para obtener todos los roles
        public List<Role> GetRoles()
        {
            return _repository.Roles.ToList();
        }

        // 1. Crear Usuario
        public void AddUser(string username, string password, string roleName)
        {
            // Verifica que el rol exista
            var role = _repository.Roles.FirstOrDefault(r => r.RoleName == roleName);
            if (role == null)
            {
                throw new Exception("El rol especificado no existe.");
            }

            // Verifica si el nombre de usuario ya existe
            if (_repository.Users.Any(u => u.Username == username))
            {
                throw new Exception("El nombre de usuario ya existe.");
            }

            // Crea un nuevo usuario
            var user = new User
            {
                UserID = _repository.Users.Count + 1, // Generar un nuevo ID
                Username = username,
                PasswordHash = password, // Aquí podrías aplicar un hash de contraseña seguro
                RoleID = role.RoleID,
                Role = role
            };

            _repository.Users.Add(user);
        }

        // 2. Leer Todos los Usuarios
        public List<User> GetAllUsers()
        {
            // Devuelve la lista de usuarios con sus roles
            return _repository.Users.ToList();
        }

        // 3. Leer un Usuario por ID
        public User GetUserById(int userId)
        {
            // Busca el usuario por ID
            var user = _repository.Users.FirstOrDefault(u => u.UserID == userId);
            if (user == null)
            {
                throw new Exception("Usuario no encontrado.");
            }

            return user;
        }

        // 4. Actualizar Usuario
        public void UpdateUser(int userId, string newUsername, string newPassword, string newRoleName)
        {
            // Busca el usuario
            var user = _repository.Users.FirstOrDefault(u => u.UserID == userId);
            if (user == null)
            {
                throw new Exception("Usuario no encontrado.");
            }

            // Busca el rol
            var role = _repository.Roles.FirstOrDefault(r => r.RoleName == newRoleName);
            if (role == null)
            {
                throw new Exception("El rol especificado no existe.");
            }

            // Actualiza los datos del usuario
            user.Username = newUsername;
            user.PasswordHash = newPassword; // Aquí también deberías aplicar un hash seguro
            user.RoleID = role.RoleID;
            user.Role = role;
        }

        // 5. Eliminar Usuario
        public void DeleteUser(int userId)
        {
            // Busca el usuario
            var user = _repository.Users.FirstOrDefault(u => u.UserID == userId);
            if (user == null)
            {
                throw new Exception("Usuario no encontrado.");
            }

            // Elimina el usuario
            _repository.Users.Remove(user);
        }
    }
}
