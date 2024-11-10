using Microsoft.Maui.Storage;
using SistemaGestionPersonal.Controller;

namespace SistemaGestionPersonal.Views;

public partial class CreateUserPage : ContentPage
{
    private readonly UserController _userController;

    public CreateUserPage()
    {
        InitializeComponent();
        _userController = new UserController(); // Dependiendo de tu configuración, puedes inyectar esto
    }

    private async void OnCreateUserButtonClicked(object sender, EventArgs e)
    {
        string username = UsernameEntry.Text;
        string password = PasswordEntry.Text;
        string role = RolePicker.SelectedItem.ToString();

        bool success = await _userController.CreateUser(username, password, role);

        if (success)
        {
            await DisplayAlert("Éxito", "El usuario ha sido creado exitosamente.", "OK");
            // Navegar de regreso o limpiar la vista si es necesario
        }
        else
        {
            await DisplayAlert("Error", "El usuario ya existe.", "OK");
        }
    }

    private async void OnBackButtonClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//EmployeeListPage");
    }

}