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

        public PayrollPage() : this(GlobalRepository.Repository, 0) // Pasa el ID del empleado actual desde la autenticaci�n
        {
        }

        public PayrollPage(InMemoryRepository repository, int employeeId)
        {
            InitializeComponent();
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _currentEmployeeId = employeeId;

            LoadPayroll();
        }

        // Cargar n�minas del empleado actual
        private void LoadPayroll()
        {
            // Filtrar las n�minas por el empleado actual
            var payrolls = _repository.Nominas
                .Where(n => n.IdEmpleado == _currentEmployeeId)
                .OrderByDescending(n => n.FechaPago)
                .ToList();

            if (!payrolls.Any())
            {
                DisplayAlert("Informaci�n", "No se encontraron n�minas para el empleado actual.", "OK");
            }

            PayrollListView.ItemsSource = payrolls;
        }

        // Mostrar detalles de la n�mina seleccionada
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
