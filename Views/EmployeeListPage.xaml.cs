namespace SistemaGestionPersonal.Views;

public partial class EmployeeListPage : ContentPage
{
    public EmployeeListPage()
    {
        InitializeComponent();
    }

    private async void OnCreateUserButtonClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(CreateUserPage));
    }
}