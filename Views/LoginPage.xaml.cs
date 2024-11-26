using SistemaGestionPersonal.Controller;
using SistemaGestionPersonal.Models;

namespace SistemaGestionPersonal.Views;

public partial class LoginPage : ContentPage
{
    private readonly LoginController _loginController;

    public LoginPage()
    {
        InitializeComponent();
        _loginController = new LoginController();

        // Establecer el enfoque en el campo de usuario cuando la página se cargue
        UsernameEntry.Focus();
    }

    private async void OnLoginButtonClicked(object sender, EventArgs e)
    {
        string username = UsernameEntry.Text?.Trim();
        string password = PasswordEntry.Text?.Trim();

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            await DisplayAlert("Error", "Por favor, ingresa tanto el nombre de usuario como la contraseña.", "OK");
            return;
        }

        User user = _loginController.AuthenticateUser(username, password);

        if (user != null)
        {
            if (user.Role == "Admin")
            {
                await DisplayAlert("Bienvenido", "Has iniciado sesión como Administrador.", "OK");
                await Shell.Current.GoToAsync(nameof(EmployeeListPage));
            }
            else if (user.Role == "Employee")
            {
                await DisplayAlert("Bienvenido", "Has iniciado sesión como Empleado.", "OK");
                await Shell.Current.GoToAsync(nameof(EmployeeHomePage));
            }
        }
        else
        {
            await DisplayAlert("Error", "Nombre de usuario o contraseña incorrectos.", "OK");
        }
    }
}
