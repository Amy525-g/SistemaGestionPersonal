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

        // Navegar a la p�gina de n�mina
        private async void OnViewPayrollClicked(object sender, EventArgs e)
        {
            try
            {
                // Navegar a la p�gina de n�mina
                await Shell.Current.GoToAsync("//PayrollPage");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudo navegar a la p�gina de n�mina: {ex.Message}", "OK");
            }
        }

        // Navegar a la p�gina de evaluaciones de desempe�o
        private async void OnViewPerformanceClicked(object sender, EventArgs e)
        {
            try
            {
                // Navegar a la p�gina de evaluaciones
                await Shell.Current.GoToAsync("//PerformancePage");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudo navegar a la p�gina de evaluaciones: {ex.Message}", "OK");
            }
        }

        // Bot�n de regresar
        private async void OnBackButtonClicked(object sender, EventArgs e)
        {
            try
            {
                // Navegar a la p�gina de inicio de sesi�n
                await Shell.Current.GoToAsync("//LoginPage");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudo navegar a la p�gina de inicio de sesi�n: {ex.Message}", "OK");
            }
        }
    }
}
