using SistemaGestionPersonal.Controller;
using SistemaGestionPersonal.Data;
using SistemaGestionPersonal.Models;
using static SistemaGestionPersonal.Data.InMemoryRepository;

namespace SistemaGestionPersonal.Views;

public partial class ManageNominasPage : ContentPage
{
    private readonly NominaController _nominaController;
    private readonly UserController _userController;
    private readonly InMemoryRepository _repository;
    private Nomina? _selectedNomina;

    // Constructor sin parámetros
    public ManageNominasPage() : this(new NominaController(new InMemoryRepository()), new InMemoryRepository(), new UserController(new InMemoryRepository()))
    {
    }

    // Constructor principal
    public ManageNominasPage(NominaController nominaController, InMemoryRepository repository, UserController userController)
    {
        InitializeComponent();
        _nominaController = nominaController;
        _userController = userController;
        _repository = repository;

        LoadEmployees();
        LoadNominas();
    }

    // Cargar empleados en el Picker
    private void LoadEmployees()
    {
        var employees = _repository.Empleados.ToList();
        EmployeePicker.ItemsSource = employees;
        EmployeePicker.ItemDisplayBinding = new Binding("Nombre");
    }

    // Crear Nómina
    private async void OnCreateNominaClicked(object sender, EventArgs e)
    {
        var selectedEmployee = EmployeePicker.SelectedItem as Empleado;
        if (selectedEmployee == null || string.IsNullOrEmpty(SalarioBrutoEntry.Text) || string.IsNullOrEmpty(DeduccionesEntry.Text))
        {
            await DisplayAlert("Error", "Por favor, llena todos los campos.", "OK");
            return;
        }

        try
        {
            _nominaController.AddNomina(
                selectedEmployee.IdEmpleado,
                decimal.Parse(SalarioBrutoEntry.Text),
                decimal.Parse(DeduccionesEntry.Text),
                FechaPagoPicker.Date
            );

            await DisplayAlert("Éxito", "Nómina creada exitosamente.", "OK");
            ClearForm();
            LoadNominas();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

    // Cargar Nóminas
    private void LoadNominas()
    {
        var nominas = _repository.Nominas.ToList();
        NominaListView.ItemsSource = nominas;
    }

    // Seleccionar Nómina
    private void OnNominaSelected(object sender, SelectedItemChangedEventArgs e)
    {
        _selectedNomina = e.SelectedItem as Nomina;
        if (_selectedNomina != null)
        {
            EmployeePicker.SelectedItem = _repository.Empleados.FirstOrDefault(emp => emp.IdEmpleado == _selectedNomina.IdEmpleado);
            SalarioBrutoEntry.Text = _selectedNomina.SalarioBruto.ToString();
            DeduccionesEntry.Text = _selectedNomina.Deducciones.ToString();
            FechaPagoPicker.Date = _selectedNomina.FechaPago;
        }
    }

    // Editar Nómina
    private async void OnEditNominaClicked(object sender, EventArgs e)
    {
        if (_selectedNomina == null)
        {
            await DisplayAlert("Error", "Selecciona una nómina para editar.", "OK");
            return;
        }

        var selectedEmployee = EmployeePicker.SelectedItem as Empleado;
        if (selectedEmployee == null || string.IsNullOrEmpty(SalarioBrutoEntry.Text) || string.IsNullOrEmpty(DeduccionesEntry.Text))
        {
            await DisplayAlert("Error", "Por favor, llena todos los campos.", "OK");
            return;
        }

        try
        {
            _nominaController.UpdateNomina(
                _selectedNomina.IdNomina,
                decimal.Parse(SalarioBrutoEntry.Text),
                decimal.Parse(DeduccionesEntry.Text),
                FechaPagoPicker.Date
            );

            await DisplayAlert("Éxito", "Nómina actualizada exitosamente.", "OK");
            ClearForm();
            LoadNominas();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

    // Eliminar Nómina
    private async void OnDeleteNominaClicked(object sender, EventArgs e)
    {
        if (_selectedNomina == null)
        {
            await DisplayAlert("Error", "Selecciona una nómina para eliminar.", "OK");
            return;
        }

        bool confirm = await DisplayAlert("Confirmación", "¿Estás seguro de que deseas eliminar esta nómina?", "Sí", "No");
        if (confirm)
        {
            try
            {
                _nominaController.DeleteNomina(_selectedNomina.IdNomina);
                await DisplayAlert("Éxito", "Nómina eliminada exitosamente.", "OK");
                ClearForm();
                LoadNominas();
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
        SalarioBrutoEntry.Text = string.Empty;
        DeduccionesEntry.Text = string.Empty;
        FechaPagoPicker.Date = DateTime.Now;
        _selectedNomina = null;
    }
    private async void OnBackButtonClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//EmployeeListPage");
    }
}
