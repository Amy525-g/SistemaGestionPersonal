using SistemaGestionPersonal.Controller;
using SistemaGestionPersonal.Data;
using SistemaGestionPersonal.Models;
using MySql.Data.MySqlClient;

namespace SistemaGestionPersonal.Views;

public partial class ManageNominasPage : ContentPage
{
    private readonly MySqlConnectionProvider _connectionProvider;
    private Nomina? _selectedNomina;

    public ManageNominasPage()
    {
        InitializeComponent();
        _connectionProvider = new MySqlConnectionProvider();

        LoadEmployees();
        LoadNominas();
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

            EmployeePicker.ItemsSource = employees;
            EmployeePicker.ItemDisplayBinding = new Binding("Nombre");
        }
        catch (Exception ex)
        {
            DisplayAlert("Error", $"Error al cargar empleados: {ex.Message}", "OK");
        }
    }


    // Crear N�mina
    private async void OnCreateNominaClicked(object sender, EventArgs e)
    {
        var selectedEmployee = EmployeePicker.SelectedItem as Empleado;

        if (selectedEmployee == null || string.IsNullOrEmpty(SalarioBrutoEntry.Text) || string.IsNullOrEmpty(DeduccionesEntry.Text))
        {
            await DisplayAlert("Error", "Por favor, llena todos los campos.", "OK");
            return;
        }

        if (!decimal.TryParse(SalarioBrutoEntry.Text, out decimal salarioBruto) || !decimal.TryParse(DeduccionesEntry.Text, out decimal deducciones))
        {
            await DisplayAlert("Error", "Por favor, ingresa valores num�ricos v�lidos para el salario y las deducciones.", "OK");
            return;
        }

        if (salarioBruto < deducciones)
        {
            await DisplayAlert("Error", "Las deducciones no pueden superar el salario bruto.", "OK");
            return;
        }

        try
        {
            using var connection = _connectionProvider.GetConnection();
            connection.Open();

            string query = @"
                INSERT INTO Nomina (IdEmpleado, SalarioBruto, Deducciones, SalarioNeto, FechaPago)
                VALUES (@IdEmpleado, @SalarioBruto, @Deducciones, @SalarioNeto, @FechaPago)";

            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@IdEmpleado", selectedEmployee.IdEmpleado);
            command.Parameters.AddWithValue("@SalarioBruto", salarioBruto);
            command.Parameters.AddWithValue("@Deducciones", deducciones);
            command.Parameters.AddWithValue("@SalarioNeto", salarioBruto - deducciones);
            command.Parameters.AddWithValue("@FechaPago", FechaPagoPicker.Date);

            command.ExecuteNonQuery();

            await DisplayAlert("�xito", "N�mina creada exitosamente.", "OK");
            ClearForm();
            LoadNominas();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error al crear la n�mina: {ex.Message}", "OK");
        }
    }

    // Cargar N�minas
    private void LoadNominas()
    {
        try
        {
            using var connection = _connectionProvider.GetConnection();
            connection.Open();

            string query = @"
                SELECT n.IdNomina, n.SalarioBruto, n.Deducciones, n.SalarioNeto, n.FechaPago, e.Nombre AS EmpleadoNombre
                FROM Nomina n
                JOIN Empleado e ON n.IdEmpleado = e.IdEmpleado";

            using var command = new MySqlCommand(query, connection);
            using var reader = command.ExecuteReader();

            var nominas = new List<dynamic>();

            while (reader.Read())
            {
                nominas.Add(new
                {
                    IdNomina = reader.GetInt32("IdNomina"),
                    EmpleadoNombre = reader.GetString("EmpleadoNombre"),
                    SalarioBruto = reader.GetDecimal("SalarioBruto"),
                    Deducciones = reader.GetDecimal("Deducciones"),
                    SalarioNeto = reader.GetDecimal("SalarioNeto"),
                    FechaPago = reader.GetDateTime("FechaPago")
                });
            }

            NominaListView.ItemsSource = nominas;
        }
        catch (Exception ex)
        {
            DisplayAlert("Error", $"Error al cargar n�minas: {ex.Message}", "OK");
        }
    }

    // Seleccionar N�mina
    private void OnNominaSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem == null) return;

        var selectedNomina = e.SelectedItem as dynamic;

        _selectedNomina = new Nomina
        {
            IdNomina = selectedNomina.IdNomina,
            SalarioBruto = selectedNomina.SalarioBruto,
            Deducciones = selectedNomina.Deducciones,
            FechaPago = selectedNomina.FechaPago
        };

        SalarioBrutoEntry.Text = selectedNomina.SalarioBruto.ToString();
        DeduccionesEntry.Text = selectedNomina.Deducciones.ToString();
        FechaPagoPicker.Date = selectedNomina.FechaPago;
        EmployeePicker.SelectedItem = selectedNomina.EmpleadoNombre;
    }

    // Editar N�mina
    private async void OnEditNominaClicked(object sender, EventArgs e)
    {
        if (_selectedNomina == null)
        {
            await DisplayAlert("Error", "Selecciona una n�mina para editar.", "OK");
            return;
        }

        // Validaci�n similar a OnCreateNominaClicked...

        try
        {
            // Similar l�gica que en OnCreateNominaClicked, pero con UPDATE
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

    // Eliminar N�mina
    private async void OnDeleteNominaClicked(object sender, EventArgs e)
    {
        if (_selectedNomina == null)
        {
            await DisplayAlert("Error", "Selecciona una n�mina para eliminar.", "OK");
            return;
        }

        bool confirm = await DisplayAlert("Confirmaci�n", "�Est�s seguro de que deseas eliminar esta n�mina?", "S�", "No");
        if (confirm)
        {
            try
            {
                using var connection = _connectionProvider.GetConnection();
                connection.Open();

                string query = "DELETE FROM Nomina WHERE IdNomina = @IdNomina";
                using var command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@IdNomina", _selectedNomina.IdNomina);

                command.ExecuteNonQuery();

                await DisplayAlert("�xito", "N�mina eliminada exitosamente.", "OK");
                ClearForm();
                LoadNominas();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al eliminar la n�mina: {ex.Message}", "OK");
            }
        }
    }

    // Limpiar Formulario
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
