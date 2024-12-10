using SistemaGestionPersonal.Controller;
using SistemaGestionPersonal.Data;
using SistemaGestionPersonal.Models;

namespace SistemaGestionPersonal.Views;

public partial class ManageContractsPage : ContentPage
{
    private readonly ContractController _contractController;
    private readonly UserController _userController;
    private readonly InMemoryRepository _repository;
    private Contrato _selectedContract;

    // Constructor sin parámetros
    public ManageContractsPage() : this(new ContractController(GlobalRepository.Repository), new UserController(GlobalRepository.Repository))
    {
    }

    public ManageContractsPage(ContractController contractController, UserController userController)
    {
        InitializeComponent();
        _contractController = contractController;
        _userController = userController;
        _repository = GlobalRepository.Repository;

        LoadEmployees();
        LoadContracts();
    }

    // Cargar empleados en el Picker
    private void LoadEmployees()
    {
        var employees = _repository.Empleados
            .Where(e => _repository.Users.Any(u => u.UserID == e.UserID && u.Role.RoleName == "Employee"))
            .ToList();

        if (!employees.Any())
        {
            DisplayAlert("Advertencia", "No se encontraron empleados disponibles.", "OK");
        }

        EmployeePicker.ItemsSource = employees;
        EmployeePicker.ItemDisplayBinding = new Binding("Nombre");
    }

    // Crear Contrato
    private async void OnCreateContractClicked(object sender, EventArgs e)
    {
        var selectedEmployee = EmployeePicker.SelectedItem as Empleado;
        if (selectedEmployee == null || ContractTypePicker.SelectedItem == null)
        {
            await DisplayAlert("Error", "Por favor, llena todos los campos.", "OK");
            return;
        }

        if (StartDatePicker.Date >= EndDatePicker.Date)
        {
            await DisplayAlert("Error", "La fecha de inicio debe ser anterior a la fecha de fin.", "OK");
            return;
        }

        try
        {
            _contractController.AddContract(
                selectedEmployee.IdEmpleado,
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
        var contracts = _repository.Contratos
            .Select(c => new
            {
                c.IdContrato,
                EmpleadoNombre = _repository.Empleados.FirstOrDefault(e => e.IdEmpleado == c.IdEmpleado)?.Nombre ?? "No encontrado",
                c.TipoContrato,
                c.FechaInicio,
                c.FechaFin
            })
            .ToList();

        ContractListView.ItemsSource = contracts;
    }

    // Seleccionar Contrato
    private void OnContractSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem is not Contrato selectedContract)
        {
            _selectedContract = null;
            return;
        }

        _selectedContract = selectedContract;

        var selectedEmployee = _repository.Empleados.FirstOrDefault(e => e.IdEmpleado == _selectedContract.IdEmpleado);
        EmployeePicker.SelectedItem = selectedEmployee;
        StartDatePicker.Date = _selectedContract.FechaInicio;
        EndDatePicker.Date = _selectedContract.FechaFin;
        ContractTypePicker.SelectedItem = _selectedContract.TipoContrato;
    }

    // Editar Contrato
    private async void OnEditContractClicked(object sender, EventArgs e)
    {
        if (_selectedContract == null)
        {
            await DisplayAlert("Error", "Selecciona un contrato para editar.", "OK");
            return;
        }

        var selectedEmployee = EmployeePicker.SelectedItem as Empleado;
        if (selectedEmployee == null || ContractTypePicker.SelectedItem == null)
        {
            await DisplayAlert("Error", "Por favor, llena todos los campos.", "OK");
            return;
        }

        if (StartDatePicker.Date >= EndDatePicker.Date)
        {
            await DisplayAlert("Error", "La fecha de inicio debe ser anterior a la fecha de fin.", "OK");
            return;
        }

        try
        {
            _contractController.UpdateContract(
                _selectedContract.IdContrato,
                selectedEmployee.IdEmpleado,
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

        if (_selectedContract == null)
        {
            await DisplayAlert("Error", "Selecciona un contrato para eliminar.", "OK");
            return;
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
