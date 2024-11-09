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
    }

    private async void OnLoginButtonClicked(object sender, EventArgs e)
    {
        string username = UsernameEntry.Text;
        string password = PasswordEntry.Text;

        User user = _loginController.AuthenticateUser(username, password);
        if (user != null)
        {
            if (user.Role == "Admin")
            {
                await DisplayAlert("Bienvenido", "Has iniciado sesi�n como Administrador", "OK");
                await Shell.Current.GoToAsync(nameof(EmployeeListPage));
                // Navegar a la p�gina de administrador
            }
            else if (user.Role == "Employee")
            {
                await DisplayAlert("Bienvenido", "Has iniciado sesi�n como Empleado", "OK");
                await Shell.Current.GoToAsync(nameof(EmployeeHomePage));
                // Navegar a la p�gina de empleado
            }
        }
        else
        {
            await DisplayAlert("Error", "Nombre de usuario o contrase�a incorrectos", "OK");
        }
    }
}