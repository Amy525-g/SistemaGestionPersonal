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

        // Navegar a la p�gina de n�mina
        private async void OnViewPayrollClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//PayrollPage");
        }

        // Navegar a la p�gina de evaluaciones de desempe�o
        private async void OnViewPerformanceClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//PerformancePage");
        }

        // Bot�n de regresar
        private async void OnBackButtonClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}
