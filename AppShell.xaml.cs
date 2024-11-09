using SistemaGestionPersonal.Views;

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

        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Navegar automáticamente a LoginPage al iniciar la aplicación
            await Shell.Current.GoToAsync("//LoginPage");
        }

    }
}
