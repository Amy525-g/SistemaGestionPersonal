using SistemaGestionPersonal.Data;
using SistemaGestionPersonal.Models;
using System;
using System.Linq;

namespace SistemaGestionPersonal.Views
{
    public partial class PerformancePage : ContentPage
    {
        private readonly InMemoryRepository _repository;
        private readonly int _currentEmployeeId;

        public PerformancePage() : this(new InMemoryRepository(), 0) // Ajusta el ID del empleado actual
        {
        }

        public PerformancePage(InMemoryRepository repository, int employeeId)
        {
            InitializeComponent();
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _currentEmployeeId = employeeId;

            LoadEvaluations();
        }

        // Cargar evaluaciones del empleado actual
        private void LoadEvaluations()
        {
            var evaluations = _repository.EvaluacionesDesempeno
                .Where(e => e.IdEmpleado == _currentEmployeeId)
                .OrderByDescending(e => e.FechaEvaluacion)
                .ToList();

            EvaluationListView.ItemsSource = evaluations;
        }

        // Mostrar detalles de la evaluaci�n seleccionada
        private async void OnEvaluationSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var selectedEvaluation = e.SelectedItem as EvaluacionDesempeno;
            if (selectedEvaluation != null)
            {
                await DisplayAlert("Detalles de Evaluaci�n",
                    $"Fecha: {selectedEvaluation.FechaEvaluacion:dd/MM/yyyy}\n" +
                    $"Puntuaci�n: {selectedEvaluation.Puntuacion}\n" +
                    $"Comentarios: {selectedEvaluation.Comentarios ?? "Sin comentarios"}",
                    "OK");

                // Deseleccionar la evaluaci�n despu�s de mostrar los detalles
                EvaluationListView.SelectedItem = null;
            }
        }

        // Bot�n para regresar
        private async void OnBackButtonClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//EmployeeHomePage");
        }
    }
}
