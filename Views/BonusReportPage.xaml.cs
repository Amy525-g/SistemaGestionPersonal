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

            // Usar el repositorio global para los datos
            _bonusController = new BonusController(GlobalRepository.Repository);

            // Calcular y cargar los bonos en la lista
            CalculateAndLoadBonuses();
        }

        private void CalculateAndLoadBonuses()
        {
            try
            {
                // Calcular bonos para todos los empleados
                _bonusController.CalculateAndAssignBonuses();

                // Obtener los bonos calculados
                var bonuses = _bonusController.GetBonuses();

                if (!bonuses.Any())
                {
                    DisplayAlert("Información", "No hay bonos disponibles.", "OK");
                    return;
                }

                // Mostrar los bonos en el ListView
                BonusesListView.ItemsSource = bonuses.Select(b => new
                {
                    NombreEmpleado = $"{b.Empleado.Nombre} {b.Empleado.Apellido}",
                    Detalle = $"Categoría: {b.Categoria}, Porcentaje: {b.Porcentaje}%, Monto: {b.MontoTotal:C}, Fecha: {b.FechaAsignacion:dd/MM/yyyy}",
                    Categoria = b.Categoria,
                    Porcentaje = b.Porcentaje,
                    MontoTotal = b.MontoTotal,
                    FechaAsignacion = b.FechaAsignacion
                }).ToList();
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", $"Ocurrió un error al calcular los bonos: {ex.Message}", "OK");
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
