using SistemaGestionPersonal.Data;
using SistemaGestionPersonal.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace SistemaGestionPersonal.Views;

public partial class PerformanceEvaluationPage : ContentPage
{
    private readonly MySqlConnectionProvider _connectionProvider;
    private EvaluacionDesempeno? _selectedEvaluation; // Variable para manejar la evaluación seleccionada

    public PerformanceEvaluationPage()
    {
        InitializeComponent();
        _connectionProvider = new MySqlConnectionProvider();

        LoadEmployees();
        LoadEvaluations();
    }

    // Cargar empleados en el Picker
    private void LoadEmployees()
    {
        try
        {
            using var connection = _connectionProvider.GetConnection();
            connection.Open();

            string query = "SELECT IdEmpleado, Nombre FROM Empleado";
            using var command = new MySqlCommand(query, connection);
            using var reader = command.ExecuteReader();

            var employees = new List<Empleado>();
            while (reader.Read())
            {
                employees.Add(new Empleado
                {
                    IdEmpleado = reader.GetInt32("IdEmpleado"),
                    Nombre = reader.GetString("Nombre")
                });
            }

            EmployeePicker.ItemsSource = employees;
            EmployeePicker.ItemDisplayBinding = new Binding("Nombre");
        }
        catch (MySqlException ex)
        {
            DisplayAlert("Error", $"Error al cargar empleados: {ex.Message}", "OK");
        }
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
            using var connection = _connectionProvider.GetConnection();
            connection.Open();

            string query = @"
                INSERT INTO EvaluacionDesempeno (IdEmpleado, FechaEvaluacion, Puntuacion, Comentarios)
                VALUES (@IdEmpleado, @FechaEvaluacion, @Puntuacion, @Comentarios)";

            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@IdEmpleado", selectedEmployee.IdEmpleado);
            command.Parameters.AddWithValue("@FechaEvaluacion", EvaluationDatePicker.Date);
            command.Parameters.AddWithValue("@Puntuacion", (int)ScoreSlider.Value);
            command.Parameters.AddWithValue("@Comentarios", CommentsEditor.Text ?? string.Empty);

            command.ExecuteNonQuery();

            await DisplayAlert("Éxito", "Evaluación guardada exitosamente.", "OK");
            ClearForm();
            LoadEvaluations();
        }
        catch (MySqlException ex)
        {
            await DisplayAlert("Error", $"Error al guardar la evaluación: {ex.Message}", "OK");
        }
    }

    // Cargar Evaluaciones
    private void LoadEvaluations()
    {
        try
        {
            using var connection = _connectionProvider.GetConnection();
            connection.Open();

            string query = @"
            SELECT ed.IdEvaluacion, ed.FechaEvaluacion, ed.Puntuacion, ed.Comentarios, e.Nombre AS EmpleadoNombre
            FROM EvaluacionDesempeno ed
            JOIN Empleado e ON ed.IdEmpleado = e.IdEmpleado";

            using var command = new MySqlCommand(query, connection);
            using var reader = command.ExecuteReader();

            var evaluations = new List<EvaluationData>();
            while (reader.Read())
            {
                evaluations.Add(new EvaluationData
                {
                    IdEvaluacion = reader.GetInt32("IdEvaluacion"),
                    FechaEvaluacion = reader.GetDateTime("FechaEvaluacion"),
                    Puntuacion = reader.GetInt32("Puntuacion"),
                    Comentarios = reader.IsDBNull("Comentarios") ? string.Empty : reader.GetString("Comentarios"),
                    EmpleadoNombre = reader.GetString("EmpleadoNombre")
                });
            }

            if (evaluations.Count == 0)
            {
                DisplayAlert("Información", "No hay evaluaciones disponibles.", "OK");
            }

            EvaluationListView.ItemsSource = evaluations;
        }
        catch (MySqlException ex)
        {
            DisplayAlert("Error", $"Error al cargar evaluaciones: {ex.Message}", "OK");
        }
        catch (Exception ex)
        {
            DisplayAlert("Error", $"Ocurrió un error inesperado: {ex.Message}", "OK");
        }
    }


    // Seleccionar Evaluación
    private async void OnEvaluationSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem is not EvaluationData selectedEvaluation) return;

        await DisplayAlert("Detalles de Evaluación",
            $"Fecha: {selectedEvaluation.FechaEvaluacion:dd/MM/yyyy}\n" +
            $"Puntuación: {selectedEvaluation.Puntuacion}\n" +
            $"Comentarios: {selectedEvaluation.Comentarios}",
            "OK");

        EvaluationListView.SelectedItem = null;
    }


    // Limpiar el formulario
    private void ClearForm()
    {
        EmployeePicker.SelectedItem = null;
        EvaluationDatePicker.Date = DateTime.Now;
        ScoreSlider.Value = 5;
        CommentsEditor.Text = string.Empty;
        _selectedEvaluation = null;
    }

    // Botón para regresar
    private async void OnBackButtonClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//EmployeeListPage");
    }
}
