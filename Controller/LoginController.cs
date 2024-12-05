using SistemaGestionPersonal.Data;
using SistemaGestionPersonal.Models;
using System;
using System.Linq;

namespace SistemaGestionPersonal.Controller
{
    public class LoginController
    {
        private readonly InMemoryRepository _repository;

        public LoginController(InMemoryRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public User AuthenticateUser(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("El nombre de usuario y la contraseña son obligatorios.");
            }

            var user = _repository.Users.FirstOrDefault(u => u.Username == username && u.PasswordHash == password);

            return user ?? throw new UnauthorizedAccessException("Nombre de usuario o contraseña incorrectos.");
        }
    }
}
