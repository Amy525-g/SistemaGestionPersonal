using SistemaGestionPersonal.Controller;
using SistemaGestionPersonal.Data;
using SistemaGestionPersonal.Models;

namespace SistemaGestionPersonal.Views;

public partial class PerformanceEvaluationPage : ContentPage
{
    private readonly EvaluationController _evaluationController;
    private readonly InMemoryRepository _repository;
    private EvaluacionDesempeno _selectedEvaluation; // Variable para manejar la evaluación seleccionada

    public PerformanceEvaluationPage() : this(new EvaluationController(new InMemoryRepository()), new InMemoryRepository())
    {
    }

    public PerformanceEvaluationPage(EvaluationController evaluationController, InMemoryRepository repository)
    {
        InitializeComponent();
        _evaluationController = evaluationController;
        _repository = repository;

        LoadEmployees();
        LoadEvaluations();
    }

    // Cargar empleados en el Picker
    private void LoadEmployees()
    {
        EmployeePicker.ItemsSource = _repository.Empleados.ToList();
        EmployeePicker.ItemDisplayBinding = new Binding("Nombre");
    }

    // Guardar Evaluación
    private async void OnSaveEvaluationClicked(object sender, EventArgs e)
    {
        var selectedEmployee = EmployeePicker.SelectedItem as Empleado;
        if (selectedEmployee == null)
        {
            await DisplayAlert("Error", "Selecciona un empleado.", "OK");
            return;
        }

        try
        {
            _evaluationController.AddEvaluation(
                selectedEmployee.IdEmpleado,
                EvaluationDatePicker.Date,
                (int)ScoreSlider.Value,
                CommentsEditor.Text
            );

            await DisplayAlert("Éxito", "Evaluación guardada exitosamente.", "OK");
            ClearForm();
            LoadEvaluations();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

    // Cargar Evaluaciones
    private void LoadEvaluations()
    {
        EvaluationListView.ItemsSource = _repository.EvaluacionesDesempeno.ToList();
    }

    // Seleccionar Evaluación
    private void OnEvaluationSelected(object sender, SelectedItemChangedEventArgs e)
    {
        _selectedEvaluation = e.SelectedItem as EvaluacionDesempeno;

        if (_selectedEvaluation != null)
        {
            var selectedEmployee = _repository.Empleados.FirstOrDefault(emp => emp.IdEmpleado == _selectedEvaluation.IdEmpleado);
            EmployeePicker.SelectedItem = selectedEmployee;
            EvaluationDatePicker.Date = _selectedEvaluation.FechaEvaluacion;
            ScoreSlider.Value = _selectedEvaluation.Puntuacion;
            CommentsEditor.Text = _selectedEvaluation.Comentarios;
        }
    }

    // Limpiar el formulario
    private void ClearForm()
    {
        EmployeePicker.SelectedItem = null;
        EvaluationDatePicker.Date = DateTime.Now;
        ScoreSlider.Value = 1;
        CommentsEditor.Text = string.Empty;
        _selectedEvaluation = null;
    }

    // Botón para regresar
    private async void OnBackButtonClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//EmployeeListPage");
    }
}
