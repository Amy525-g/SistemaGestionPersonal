using SistemaGestionPersonal.Controller;
using SistemaGestionPersonal.Data;
using System;

namespace SistemaGestionPersonal.Views
{
    public partial class LoginPage : ContentPage
    {
        private readonly LoginController _loginController;

        public LoginPage()
        {
            InitializeComponent();

            // Inicializar el repositorio en memoria y el controlador
            var repository = new InMemoryRepository();
            _loginController = new LoginController(repository);
        }

        private async void OnLoginButtonClicked(object sender, EventArgs e)
        {
            try
            {
                string username = UsernameEntry.Text?.Trim();
                string password = PasswordEntry.Text?.Trim();

                // Validación inicial
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    await DisplayAlert("Error", "Por favor, ingresa tanto el nombre de usuario como la contraseña.", "OK");
                    return;
                }

                // Autenticación del usuario
                var user = _loginController.AuthenticateUser(username, password);

                if (user != null)
                {
                    // Verificar el rol del usuario
                    if (user.Role.RoleName == "Admin")
                    {
                        await DisplayAlert("Éxito", $"Bienvenido, {user.Username}.", "OK");
                        // Navegar a la página de administrador
                        await Shell.Current.GoToAsync(nameof(EmployeeListPage));
                    }
                    else if (user.Role.RoleName == "Employee")
                    {
                        await DisplayAlert("Éxito", $"Bienvenido, {user.Username}.", "OK");
                        // Navegar a la página de empleado
                        await Shell.Current.GoToAsync(nameof(EmployeeHomePage));
                    }
                    else
                    {
                        await DisplayAlert("Error", "Rol no reconocido.", "OK");
                    }
                }
                else
                {
                    await DisplayAlert("Error", "Nombre de usuario o contraseña incorrectos.", "OK");
                }
            }
            catch (Exception ex)
            {
                // Capturar cualquier error y mostrar un mensaje
                await DisplayAlert("Error", $"Ocurrió un error: {ex.Message}", "OK");
            }
        }
    }
}
