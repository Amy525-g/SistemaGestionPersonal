using SistemaGestionPersonal.Data;
using SistemaGestionPersonal.Models;
using System;
using System.Linq;

namespace SistemaGestionPersonal.Views
{
    public partial class PayrollPage : ContentPage
    {
        private readonly InMemoryRepository _repository;
        private readonly int _currentEmployeeId;

        public PayrollPage() : this(new InMemoryRepository(), 0) // Ajusta para pasar el ID del empleado actual.
        {
        }

        public PayrollPage(InMemoryRepository repository, int employeeId)
        {
            InitializeComponent();
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _currentEmployeeId = employeeId;

            LoadPayroll();
            // Cargar todas las nóminas
            var payrolls = _repository.Nominas.ToList();
            PayrollListView.ItemsSource = payrolls;
        }

        // Cargar nóminas del empleado actual
        private void LoadPayroll()
        {
            // Obtener empleado autenticada
            var currentUser = _repository.Users.FirstOrDefault(u => u.Username == "John"); // Cambia según autenticación
            if (currentUser == null || currentUser.Role.RoleName != "Employee")
            {
                DisplayAlert("Error", "No se encontró el empleado autenticado.", "OK");
                return;
            }

            var payrolls = _repository.Nominas
                .Where(n => n.IdEmpleado == _currentEmployeeId)
                .OrderByDescending(n => n.FechaPago)
                .ToList();

            PayrollListView.ItemsSource = payrolls;
        }

        // Selección de nómina (opcional, puedes expandir esta funcionalidad)
        private async void OnPayrollSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var selectedPayroll = e.SelectedItem as Nomina;
            if (selectedPayroll != null)
            {
                await DisplayAlert("Detalles de Nómina",
                    $"Fecha de Pago: {selectedPayroll.FechaPago:dd/MM/yyyy}\n" +
                    $"Salario Bruto: {selectedPayroll.SalarioBruto:C}\n" +
                    $"Deducciones: {selectedPayroll.Deducciones:C}\n" +
                    $"Salario Neto: {selectedPayroll.SalarioNeto:C}",
                    "OK");
            }
        }

        // Botón para regresar
        private async void OnBackButtonClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//EmployeeHomePage");
        }
    }
}
