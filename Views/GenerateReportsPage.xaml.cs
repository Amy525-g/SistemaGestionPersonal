using SistemaGestionPersonal.Controller;
using SistemaGestionPersonal.Data;
using MySql.Data.MySqlClient;
using System;
using System.Linq;

namespace SistemaGestionPersonal.Views;

public partial class GenerateReportsPage : ContentPage
{
    private readonly MySqlConnectionProvider _connectionProvider;

    public GenerateReportsPage()
    {
        InitializeComponent();
        // Inicializar el proveedor de conexión
        _connectionProvider = new MySqlConnectionProvider();
    }

    // Generar Reporte
    private async void OnGenerateReportClicked(object sender, EventArgs e)
    {
        if (ReportTypePicker.SelectedItem == null)
        {
            await DisplayAlert("Error", "Selecciona un tipo de reporte.", "OK");
            return;
        }

        string reportType = ReportTypePicker.SelectedItem.ToString();
        string reportContent = string.Empty;

        try
        {
            switch (reportType)
            {
                case "Evaluaciones de Desempeño":
                    reportContent = GeneratePerformanceEvaluationReport();
                    break;
                case "Nóminas":
                    reportContent = GeneratePayrollReport();
                    break;
                case "Contratos":
                    reportContent = GenerateContractsReport();
                    break;
                default:
                    reportContent = "Tipo de reporte no válido.";
                    break;
            }
        }
        catch (MySqlException sqlEx)
        {
            reportContent = $"Error en la base de datos: {sqlEx.Message}";
        }
        catch (Exception ex)
        {
            reportContent = $"Error al generar el reporte: {ex.Message}";
        }

        ReportPreviewEditor.Text = reportContent;
    }

    // Generar reporte de evaluaciones de desempeño
    private string GeneratePerformanceEvaluationReport()
    {
        var evaluations = new List<string>();

        using var connection = _connectionProvider.GetConnection();
        connection.Open();

        string query = @"
            SELECT e.Nombre, ed.Puntuacion, ed.FechaEvaluacion
            FROM EvaluacionDesempeno ed
            JOIN Empleado e ON ed.IdEmpleado = e.IdEmpleado";

        using var command = new MySqlCommand(query, connection);
        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            string nombre = reader.GetString("Nombre");
            int puntuacion = reader.GetInt32("Puntuacion");
            DateTime fechaEvaluacion = reader.GetDateTime("FechaEvaluacion");

            evaluations.Add($"Empleado: {nombre}, Puntuación: {puntuacion}, Fecha: {fechaEvaluacion.ToShortDateString()}");
        }

        return evaluations.Any()
            ? string.Join(Environment.NewLine, evaluations)
            : "No hay evaluaciones de desempeño disponibles.";
    }

    // Generar reporte de nóminas
    private string GeneratePayrollReport()
    {
        var payrolls = new List<string>();

        using var connection = _connectionProvider.GetConnection();
        connection.Open();

        string query = @"
            SELECT e.Nombre, n.SalarioNeto, n.FechaPago
            FROM Nomina n
            JOIN Empleado e ON n.IdEmpleado = e.IdEmpleado";

        using var command = new MySqlCommand(query, connection);
        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            string nombre = reader.GetString("Nombre");
            decimal salarioNeto = reader.GetDecimal("SalarioNeto");
            DateTime fechaPago = reader.GetDateTime("FechaPago");

            payrolls.Add($"Empleado: {nombre}, Salario Neto: {salarioNeto:C}, Fecha de Pago: {fechaPago.ToShortDateString()}");
        }

        return payrolls.Any()
            ? string.Join(Environment.NewLine, payrolls)
            : "No hay nóminas disponibles.";
    }

    // Generar reporte de contratos
    private string GenerateContractsReport()
    {
        var contracts = new List<string>();

        using var connection = _connectionProvider.GetConnection();
        connection.Open();

        string query = @"
            SELECT e.Nombre, c.TipoContrato, c.FechaInicio, c.FechaFin
            FROM Contrato c
            JOIN Empleado e ON c.IdEmpleado = e.IdEmpleado";

        using var command = new MySqlCommand(query, connection);
        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            string nombre = reader.GetString("Nombre");
            string tipoContrato = reader.GetString("TipoContrato");
            DateTime fechaInicio = reader.GetDateTime("FechaInicio");
            DateTime fechaFin = reader.GetDateTime("FechaFin");

            contracts.Add($"Empleado: {nombre}, Tipo: {tipoContrato}, Desde: {fechaInicio.ToShortDateString()} Hasta: {fechaFin.ToShortDateString()}");
        }

        return contracts.Any()
            ? string.Join(Environment.NewLine, contracts)
            : "No hay contratos disponibles.";
    }

    // Botón para regresar
    private async void OnBackButtonClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//EmployeeListPage");
    }
}
