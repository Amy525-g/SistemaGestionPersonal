using Microsoft.Maui.Controls;
using System;

namespace SistemaGestionPersonal.Views
{
    public partial class EmployeeHomePage : ContentPage
    {
        public EmployeeHomePage()
        {
            InitializeComponent();
        }

        // Navegar a la página de nómina
        private async void OnViewPayrollClicked(object sender, EventArgs e)
        {
            try
            {
                // Navegar a la página de nómina
                await Shell.Current.GoToAsync("//PayrollPage");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudo navegar a la página de nómina: {ex.Message}", "OK");
            }
        }

        // Navegar a la página de evaluaciones de desempeño
        private async void OnViewPerformanceClicked(object sender, EventArgs e)
        {
            try
            {
                // Navegar a la página de evaluaciones
                await Shell.Current.GoToAsync("//PerformancePage");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudo navegar a la página de evaluaciones: {ex.Message}", "OK");
            }
        }

        // Botón de regresar
        private async void OnBackButtonClicked(object sender, EventArgs e)
        {
            try
            {
                // Navegar a la página de inicio de sesión
                await Shell.Current.GoToAsync("//LoginPage");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudo navegar a la página de inicio de sesión: {ex.Message}", "OK");
            }
        }
    }
}
