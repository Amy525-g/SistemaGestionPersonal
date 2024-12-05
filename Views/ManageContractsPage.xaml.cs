using SistemaGestionPersonal.Controller;
using SistemaGestionPersonal.Data;
using SistemaGestionPersonal.Models;

namespace SistemaGestionPersonal.Views;

public partial class ManageContractsPage : ContentPage
{
    private readonly ContractController _contractController;
    private readonly UserController _userController;
    private Contrato _selectedContract;

    // Constructor sin parámetros
    public ManageContractsPage() : this(new ContractController(new InMemoryRepository()), new UserController(new InMemoryRepository()))
    {
    }
    public ManageContractsPage(ContractController contractController, UserController userController)
    {
        InitializeComponent();
        _contractController = contractController;
        _userController = userController;

        LoadEmployees();
        LoadContracts();
    }

    // Cargar empleados en el Picker
    private void LoadEmployees()
    {
        var employees = _userController.GetAllUsers()
            .Where(u => u.Role.RoleName == "Employee")
            .ToList();

        EmployeePicker.ItemsSource = employees;
        EmployeePicker.ItemDisplayBinding = new Binding("Username");
    }

    // Crear Contrato
    private async void OnCreateContractClicked(object sender, EventArgs e)
    {
        var selectedEmployee = EmployeePicker.SelectedItem as User;
        if (selectedEmployee == null || StartDatePicker.Date == null || EndDatePicker.Date == null || ContractTypePicker.SelectedItem == null)
        {
            await DisplayAlert("Error", "Por favor, llena todos los campos.", "OK");
            return;
        }

        try
        {
            _contractController.AddContract(
            selectedEmployee.UserID,
            StartDatePicker.Date,
            EndDatePicker.Date,
            ContractTypePicker.SelectedItem.ToString()
            );

            await DisplayAlert("Éxito", "Contrato creado exitosamente.", "OK");
            ClearForm();
            LoadContracts();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

    // Cargar contratos en el ListView
    private void LoadContracts()
    {
        var contracts = _contractController.GetAllContracts();
        ContractListView.ItemsSource = contracts;
    }

    // Seleccionar Contrato
    private void OnContractSelected(object sender, SelectedItemChangedEventArgs e)
    {
        _selectedContract = e.SelectedItem as Contrato;
        if (_selectedContract != null)
        {
            EmployeePicker.SelectedItem = _selectedContract.Empleado;
            StartDatePicker.Date = _selectedContract.FechaInicio;
            EndDatePicker.Date = _selectedContract.FechaFin;
            ContractTypePicker.SelectedItem = _selectedContract.TipoContrato;
        }
    }

    // Editar Contrato
    private async void OnEditContractClicked(object sender, EventArgs e)
    {
        if (_selectedContract == null)
        {
            await DisplayAlert("Error", "Selecciona un contrato para editar.", "OK");
            return;
        }

        var selectedEmployee = EmployeePicker.SelectedItem as User;
        if (selectedEmployee == null || StartDatePicker.Date == null || EndDatePicker.Date == null || ContractTypePicker.SelectedItem == null)
        {
            await DisplayAlert("Error", "Por favor, llena todos los campos.", "OK");
            return;
        }

        try
        {
            _contractController.UpdateContract(
                _selectedContract.IdContrato,
                selectedEmployee.UserID,
                StartDatePicker.Date,
                EndDatePicker.Date,
                ContractTypePicker.SelectedItem.ToString()
            );

            await DisplayAlert("Éxito", "Contrato actualizado exitosamente.", "OK");
            ClearForm();
            LoadContracts();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

    // Eliminar Contrato
    private async void OnDeleteContractClicked(object sender, EventArgs e)
    {
        if (_selectedContract == null)
        {
            await DisplayAlert("Error", "Selecciona un contrato para eliminar.", "OK");
            return;
        }

        bool confirm = await DisplayAlert("Confirmación", "¿Estás seguro de que deseas eliminar este contrato?", "Sí", "No");
        if (confirm)
        {
            try
            {
                _contractController.DeleteContract(_selectedContract.IdContrato);
                await DisplayAlert("Éxito", "Contrato eliminado exitosamente.", "OK");
                ClearForm();
                LoadContracts();
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
        EmployeePicker.SelectedItem = null;
        StartDatePicker.Date = DateTime.Now;
        EndDatePicker.Date = DateTime.Now;
        ContractTypePicker.SelectedItem = null;
        _selectedContract = null;
    }

    private async void OnBackButtonClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//EmployeeListPage");
    }

}
