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
            // Cargar todas las n�minas
            var payrolls = _repository.Nominas.ToList();
            PayrollListView.ItemsSource = payrolls;
        }

        // Cargar n�minas del empleado actual
        private void LoadPayroll()
        {
            // Obtener empleado autenticada
            var currentUser = _repository.Users.FirstOrDefault(u => u.Username == "John"); // Cambia seg�n autenticaci�n
            if (currentUser == null || currentUser.Role.RoleName != "Employee")
            {
                DisplayAlert("Error", "No se encontr� el empleado autenticado.", "OK");
                return;
            }

            var payrolls = _repository.Nominas
                .Where(n => n.IdEmpleado == _currentEmployeeId)
                .OrderByDescending(n => n.FechaPago)
                .ToList();

            PayrollListView.ItemsSource = payrolls;
        }

        // Selecci�n de n�mina (opcional, puedes expandir esta funcionalidad)
        private async void OnPayrollSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var selectedPayroll = e.SelectedItem as Nomina;
            if (selectedPayroll != null)
            {
                await DisplayAlert("Detalles de N�mina",
                    $"Fecha de Pago: {selectedPayroll.FechaPago:dd/MM/yyyy}\n" +
                    $"Salario Bruto: {selectedPayroll.SalarioBruto:C}\n" +
                    $"Deducciones: {selectedPayroll.Deducciones:C}\n" +
                    $"Salario Neto: {selectedPayroll.SalarioNeto:C}",
                    "OK");
            }
        }

        // Bot�n para regresar
        private async void OnBackButtonClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//EmployeeHomePage");
        }
    }
}
