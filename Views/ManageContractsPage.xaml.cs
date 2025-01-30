using SistemaGestionPersonal.Controller;
using SistemaGestionPersonal.Data;
using SistemaGestionPersonal.Models;
using MySql.Data.MySqlClient;

namespace SistemaGestionPersonal.Views;

public partial class ManageContractsPage : ContentPage
{
    private readonly ContractController _contractController;
    private readonly MySqlConnectionProvider _connectionProvider;
    private Contrato _selectedContract;

    public ManageContractsPage()
    {
        InitializeComponent();
        _connectionProvider = new MySqlConnectionProvider();
        _contractController = new ContractController(_connectionProvider);

        LoadEmployees();
        LoadContracts();
    }

    // Cargar empleados en el Picker
    private void LoadEmployees()
    {
        try
        {
            using var connection = _connectionProvider.GetConnection();
            connection.Open();

            string query = "SELECT IdEmpleado, Nombre FROM Empleado";
            using var command = new MySqlCommand(query, connection);
            using var reader = command.ExecuteReader();

            var employees = new List<Empleado>();

            while (reader.Read())
            {
                employees.Add(new Empleado
                {
                    IdEmpleado = reader.GetInt32("IdEmpleado"),
                    Nombre = reader.GetString("Nombre")
                });
            }

            if (!employees.Any())
            {
                DisplayAlert("Advertencia", "No se encontraron empleados disponibles.", "OK");
            }

            EmployeePicker.ItemsSource = employees;
            EmployeePicker.ItemDisplayBinding = new Binding("Nombre");
        }
        catch (MySqlException ex)
        {
            DisplayAlert("Error", $"Error al cargar empleados: {ex.Message}", "OK");
        }
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
        catch (MySqlException ex)
        {
            await DisplayAlert("Error", $"Error al crear contrato: {ex.Message}", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

    // Cargar contratos en el ListView
    private void LoadContracts()
    {
        try
        {
            using var connection = _connectionProvider.GetConnection();
            connection.Open();

            string query = @"
                SELECT c.IdContrato, c.TipoContrato, c.FechaInicio, c.FechaFin, e.Nombre AS EmpleadoNombre
                FROM Contrato c
                JOIN Empleado e ON c.IdEmpleado = e.IdEmpleado";

            using var command = new MySqlCommand(query, connection);
            using var reader = command.ExecuteReader();

            var contracts = new List<dynamic>();

            while (reader.Read())
            {
                contracts.Add(new
                {
                    IdContrato = reader.GetInt32("IdContrato"),
                    EmpleadoNombre = reader.GetString("EmpleadoNombre"),
                    TipoContrato = reader.GetString("TipoContrato"),
                    FechaInicio = reader.GetDateTime("FechaInicio"),
                    FechaFin = reader.GetDateTime("FechaFin")
                });
            }

            ContractListView.ItemsSource = contracts;
        }
        catch (MySqlException ex)
        {
            DisplayAlert("Error", $"Error al cargar contratos: {ex.Message}", "OK");
        }
    }

    // Seleccionar Contrato
    private void OnContractSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem == null)
        {
            _selectedContract = null;
            return;
        }

        var selectedContract = e.SelectedItem as dynamic;

        _selectedContract = new Contrato
        {
            IdContrato = selectedContract.IdContrato,
            IdEmpleado = EmployeePicker.SelectedIndex,
            TipoContrato = selectedContract.TipoContrato,
            FechaInicio = selectedContract.FechaInicio,
            FechaFin = selectedContract.FechaFin
        };

        EmployeePicker.SelectedItem = selectedContract.EmpleadoNombre;
        StartDatePicker.Date = selectedContract.FechaInicio;
        EndDatePicker.Date = selectedContract.FechaFin;
        ContractTypePicker.SelectedItem = selectedContract.TipoContrato;
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
        catch (MySqlException ex)
        {
            await DisplayAlert("Error", $"Error al actualizar contrato: {ex.Message}", "OK");
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
            catch (MySqlException ex)
            {
                await DisplayAlert("Error", $"Error al eliminar contrato: {ex.Message}", "OK");
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
