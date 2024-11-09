using SistemaGestionPersonal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaGestionPersonal.Controller
{
    public class LoginController
    {
        private readonly List<User> _users; // Puedes usar tu base de datos aquí

        public LoginController()
        {
            // Datos de ejemplo, reemplaza con tu base de datos o repositorio
            _users = new List<User>
            {
                new User { UserID = 1, Username = "Amy Cherrez", PasswordHash = "1234", Role = "Admin" },
                new User { UserID = 2, Username = "empleado", PasswordHash = "1234", Role = "Employee" }
            };
        }

        public User AuthenticateUser(string username, string password)
        {
            return _users.FirstOrDefault(u => u.Username == username && u.PasswordHash == password);
        }
    }
}