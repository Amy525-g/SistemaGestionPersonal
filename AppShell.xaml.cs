﻿using SistemaGestionPersonal.Views;

namespace SistemaGestionPersonal
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            // Redirige a LoginPage cuando se cargue AppShell
            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(CreateUserPage), typeof(CreateUserPage));
            Routing.RegisterRoute(nameof(EmployeeListPage), typeof(EmployeeListPage));
            Routing.RegisterRoute(nameof(EmployeeHomePage), typeof(EmployeeHomePage));
            Routing.RegisterRoute(nameof(ManageContractsPage), typeof(ManageContractsPage));
            Routing.RegisterRoute(nameof(ManageNominasPage), typeof(ManageNominasPage));
            Routing.RegisterRoute(nameof(PerformanceEvaluationPage), typeof(PerformanceEvaluationPage));
            Routing.RegisterRoute(nameof(GenerateReportsPage), typeof(GenerateReportsPage));
            Routing.RegisterRoute(nameof(PayrollPage), typeof(PayrollPage));
            Routing.RegisterRoute(nameof(PerformancePage), typeof(PerformancePage));
            Routing.RegisterRoute(nameof(BonusReportPage), typeof(BonusReportPage));
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Navegar automáticamente a LoginPage al iniciar la aplicación
            await Shell.Current.GoToAsync("//LoginPage");
        }

    }
}
