using SistemaGestionPersonal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaGestionPersonal.Controller
{
    public class UserController
    {
        private readonly List<User> _users; 

        public UserController()
        {
            // Inicializa tu almacenamiento de datos aquí (base de datos)
            _users = new List<User>();
        }

        public async Task<bool> CreateUser(string username, string password, string role)
        {
            // Comprobar si el usuario ya existe
            if (_users.Any(u => u.Username == username))
            {
                return false; // El usuario ya existe
            }

            // Crea un nuevo usuario (puedes usar un hash de contraseña aquí)
            var newUser = new User
            {
                UserID = _users.Count + 1, // Generar un Id (puede ser diferente según tu base de datos)
                Username = username,
                PasswordHash = password, // Usa un hash seguro
                Role = role
            };

            _users.Add(newUser);
            // Guarda en la base de datos (si estás usando EF Core, sería await _dbContext.Users.AddAsync(newUser); await _dbContext.SaveChangesAsync();)
            return true;
        }
    }
}