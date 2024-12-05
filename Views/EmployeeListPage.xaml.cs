namespace SistemaGestionPersonal.Views;

public partial class EmployeeListPage : ContentPage
{
    public EmployeeListPage()
    {
        InitializeComponent();
    }

    // Navegar a la vista para registrar un nuevo empleado
    private async void OnCreateUserButtonClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(CreateUserPage));
    }

    // Navegar a la vista para gestionar contratos
    private async void OnManageContractsButtonClicked(object sender, EventArgs e)
    {
       await Shell.Current.GoToAsync(nameof(ManageContractsPage));
    }

    // Navegar a la vista para generar nómina
    private async void OnManageNominasPage(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(ManageNominasPage));
    }

    // Navegar a la vista para realizar evaluaciones de desempeño
    private async void OnPerformanceEvaluationButtonClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(PerformanceEvaluationPage));
    }

    // Navegar a la vista para generar informes de personal
    private async void OnGenerateReportsButtonClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(GenerateReportsPage));
    }
    private async void OnBackButtonClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//LoginPage");
    }
}

