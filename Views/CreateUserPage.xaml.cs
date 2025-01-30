using SistemaGestionPersonal.Controller;
using SistemaGestionPersonal.Models;
using SistemaGestionPersonal.Data;
using System;
using System.Linq;

namespace SistemaGestionPersonal.Views;

public partial class CreateUserPage : ContentPage
{
    private readonly UserController _userController;
    private User _selectedUser; // Usuario seleccionado para editar/eliminar

    public CreateUserPage()
    {
        InitializeComponent();

        // Instanciar el controlador con conexión MySQL
        _userController = new UserController(new MySqlConnectionProvider());

        LoadRoles(); // Cargar roles en el Picker
        LoadUsers(); // Cargar usuarios en el ListView
    }

    // Crear Usuario
    private async void OnCreateUserClicked(object sender, EventArgs e)
    {
        string username = UsernameEntry.Text?.Trim();
        string password = PasswordEntry.Text?.Trim();
        string roleName = RolePicker.SelectedItem?.ToString();

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(roleName))
        {
            await DisplayAlert("Error", "Por favor, llena todos los campos.", "OK");
            return;
        }

        try
        {
            _userController.AddUser(username, password, roleName);
            await DisplayAlert("Éxito", "Usuario creado exitosamente.", "OK");
            ClearForm();
            LoadUsers();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

    // Cargar Usuarios en el ListView
    private void LoadUsers()
    {
        try
        {
            var users = _userController.GetAllUsers();
            UserListView.ItemsSource = users; // Cargar directamente la lista de usuarios
        }
        catch (Exception ex)
        {
            DisplayAlert("Error", $"Error al cargar usuarios: {ex.Message}", "OK");
        }
    }

    // Cargar Roles en el Picker
    private void LoadRoles()
    {
        try
        {
            var roles = _userController.GetRoles();
            RolePicker.ItemsSource = roles.Select(r => r.RoleName).ToList(); // Cargar roles como strings
        }
        catch (Exception ex)
        {
            DisplayAlert("Error", $"Error al cargar roles: {ex.Message}", "OK");
        }
    }

    // Seleccionar Usuario
    private void OnUserSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem == null) return; // Asegurarse de que haya un usuario seleccionado

        _selectedUser = e.SelectedItem as User;
        if (_selectedUser != null)
        {
            UsernameEntry.Text = _selectedUser.Username;
            PasswordEntry.Text = string.Empty; // La contraseña no se muestra
            RolePicker.SelectedItem = _selectedUser.Role.RoleName;
        }
    }

    // Editar Usuario
    private async void OnEditUserClicked(object sender, EventArgs e)
    {
        if (_selectedUser == null)
        {
            await DisplayAlert("Error", "Selecciona un usuario para editar.", "OK");
            return;
        }

        string newUsername = UsernameEntry.Text?.Trim();
        string newPassword = PasswordEntry.Text?.Trim();
        string newRoleName = RolePicker.SelectedItem?.ToString();

        if (string.IsNullOrEmpty(newUsername) || string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(newRoleName))
        {
            await DisplayAlert("Error", "Por favor, llena todos los campos.", "OK");
            return;
        }

        try
        {
            _userController.UpdateUser(_selectedUser.UserID, newUsername, newPassword, newRoleName);
            await DisplayAlert("Éxito", "Usuario actualizado exitosamente.", "OK");
            ClearForm();
            LoadUsers();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

    // Eliminar Usuario
    private async void OnDeleteUserClicked(object sender, EventArgs e)
    {
        if (_selectedUser == null)
        {
            await DisplayAlert("Error", "Selecciona un usuario para eliminar.", "OK");
            return;
        }

        bool confirm = await DisplayAlert("Confirmación", "¿Estás seguro de que deseas eliminar este usuario?", "Sí", "No");
        if (confirm)
        {
            try
            {
                _userController.DeleteUser(_selectedUser.UserID);
                await DisplayAlert("Éxito", "Usuario eliminado exitosamente.", "OK");
                ClearForm();
                LoadUsers();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }

    // Limpiar el formulario
    private void ClearForm()
    {
        UsernameEntry.Text = string.Empty;
        PasswordEntry.Text = string.Empty;
        RolePicker.SelectedItem = null;
        _selectedUser = null;
    }

    private async void OnBackButtonClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//EmployeeListPage");
    }
}
