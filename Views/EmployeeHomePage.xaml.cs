using Microsoft.Maui.Controls;
using SistemaGestionPersonal.Data;
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
            await Shell.Current.GoToAsync("//PayrollPage");
        }

        // Navegar a la página de evaluaciones de desempeño
        private async void OnViewPerformanceClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//PerformancePage");
        }

        // Botón de regresar
        private async void OnBackButtonClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}
