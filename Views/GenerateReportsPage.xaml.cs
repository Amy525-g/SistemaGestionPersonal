using SistemaGestionPersonal.Data;

namespace SistemaGestionPersonal.Views;

public partial class GenerateReportsPage : ContentPage
{
    private readonly InMemoryRepository _repository;

    public GenerateReportsPage()
    {
        InitializeComponent();
        // Usar el repositorio global para garantizar la persistencia de datos
        _repository = GlobalRepository.Repository;
    }

    public GenerateReportsPage(InMemoryRepository repository)
    {
        InitializeComponent();
        // Usar el repositorio global, ignorando el parámetro del constructor
        _repository = GlobalRepository.Repository;
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

        ReportPreviewEditor.Text = reportContent;
    }

    // Generar reporte de evaluaciones de desempeño
    private string GeneratePerformanceEvaluationReport()
    {
        var evaluations = _repository.EvaluacionesDesempeno
            .Select(e => $"Empleado: {e.Empleado.Nombre}, Puntuación: {e.Puntuacion}, Fecha: {e.FechaEvaluacion.ToShortDateString()}")
            .ToList();

        return evaluations.Any()
            ? string.Join(Environment.NewLine, evaluations)
            : "No hay evaluaciones de desempeño disponibles.";
    }

    // Generar reporte de nóminas
    private string GeneratePayrollReport()
    {
        var payrolls = _repository.Nominas
            .Select(n => $"Empleado: {n.Empleado.Nombre}, Salario Neto: {n.SalarioNeto}, Fecha de Pago: {n.FechaPago.ToShortDateString()}")
            .ToList();

        return payrolls.Any()
            ? string.Join(Environment.NewLine, payrolls)
            : "No hay nóminas disponibles.";
    }

    // Generar reporte de contratos
    private string GenerateContractsReport()
    {
        var contracts = _repository.Contratos
            .Select(c => $"Empleado: {c.Empleado.Nombre}, Tipo: {c.TipoContrato}, Desde: {c.FechaInicio.ToShortDateString()} Hasta: {c.FechaFin.ToShortDateString()}")
            .ToList();

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
