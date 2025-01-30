using SistemaGestionPersonal.Controller;
using SistemaGestionPersonal.Data;
using SistemaGestionPersonal.Models;
using System;
using System.Linq;

namespace SistemaGestionPersonal.Views
{
    public partial class BonusReportPage : ContentPage
    {
        private readonly BonusController _bonusController;

        public BonusReportPage()
        {
            InitializeComponent();

            // Instanciar el controlador con el proveedor de conexión a MySQL
            _bonusController = new BonusController(new MySqlConnectionProvider());

            // Calcular y cargar los bonos en la lista
            CalculateAndLoadBonuses();
        }

        private async void CalculateAndLoadBonuses()
        {
            try
            {
                // Calcular bonos para todos los empleados
                _bonusController.CalculateAndAssignBonuses();

                // Obtener los bonos calculados
                var bonuses = _bonusController.GetBonuses();

                if (!bonuses.Any())
                {
                    await DisplayAlert("Información", "No hay bonos disponibles.", "OK");
                    return;
                }

                // Mostrar los bonos en el ListView
                BonusesListView.ItemsSource = bonuses.Select(b => new
                {
                    NombreEmpleado = b.Empleado != null ? $"{b.Empleado.Nombre} {b.Empleado.Apellido}" : "Empleado no encontrado",
                    Detalle = $"Categoría: {b.Categoria}, Porcentaje: {b.Porcentaje}%, Monto: {b.MontoTotal:C}, Fecha: {b.FechaAsignacion:dd/MM/yyyy}",
                    Categoria = b.Categoria,
                    Porcentaje = b.Porcentaje,
                    MontoTotal = b.MontoTotal,
                    FechaAsignacion = b.FechaAsignacion
                }).ToList();

            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Ocurrió un error al calcular los bonos: {ex.Message}", "OK");
            }
        }

        private async void OnBonusSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem is null)
                return;

            var selectedBonus = e.SelectedItem as dynamic;
            if (selectedBonus != null)
            {
                string mensaje = $"Empleado: {selectedBonus.NombreEmpleado}\n" +
                                 $"Categoría: {selectedBonus.Categoria}\n" +
                                 $"Porcentaje: {selectedBonus.Porcentaje}%\n" +
                                 $"Monto: {selectedBonus.MontoTotal:C}\n" +
                                 $"Fecha de Asignación: {selectedBonus.FechaAsignacion:dd/MM/yyyy}";

                await DisplayAlert("Detalles del Bono", mensaje, "OK");
            }

            // Deseleccionar el elemento después de mostrar el alerta
            BonusesListView.SelectedItem = null;
        }

        private async void OnBackButtonClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//EmployeeListPage");
        }
    }
}
