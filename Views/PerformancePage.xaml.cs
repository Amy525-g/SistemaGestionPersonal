using SistemaGestionPersonal.Data;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using SistemaGestionPersonal.Models;

namespace SistemaGestionPersonal.Views;

public partial class PerformancePage : ContentPage
{
    private readonly MySqlConnectionProvider _connectionProvider;
    private readonly int _currentEmployeeId;

    public PerformancePage(int employeeId)
    {
        InitializeComponent();
        _connectionProvider = new MySqlConnectionProvider();
        _currentEmployeeId = employeeId;

        LoadEvaluations();
    }

    // Cargar evaluaciones del empleado actual
    private void LoadEvaluations()
    {
        try
        {
            using var connection = _connectionProvider.GetConnection();
            connection.Open();

            string query = @"
            SELECT FechaEvaluacion, Puntuacion, Comentarios
            FROM EvaluacionDesempeno
            WHERE IdEmpleado = @IdEmpleado
            ORDER BY FechaEvaluacion DESC";

            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@IdEmpleado", _currentEmployeeId);

            using var reader = command.ExecuteReader();

            var evaluations = new List<EvaluationData>();
            while (reader.Read())
            {
                evaluations.Add(new EvaluationData
                {
                    FechaEvaluacion = reader.GetDateTime("FechaEvaluacion"),
                    Puntuacion = reader.GetInt32("Puntuacion"),
                    Comentarios = reader.GetString("Comentarios")
                });
            }

            if (evaluations.Count == 0)
            {
                DisplayAlert("Información", "No hay evaluaciones disponibles para este empleado.", "OK");
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


    // Mostrar detalles de la evaluación seleccionada
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

    // Botón para regresar
    private async void OnBackButtonClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//EmployeeHomePage");
    }
}

