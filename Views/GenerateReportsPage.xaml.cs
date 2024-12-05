using SistemaGestionPersonal.Data;

namespace SistemaGestionPersonal.Views;

public partial class GenerateReportsPage : ContentPage
{
    private readonly InMemoryRepository _repository;

    public GenerateReportsPage() : this(new InMemoryRepository())
    {
    }

    public GenerateReportsPage(InMemoryRepository repository)
    {
        InitializeComponent();
        _repository = repository;
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
            case "Evaluaciones de Desempe�o":
                reportContent = GeneratePerformanceEvaluationReport();
                break;
            case "N�minas":
                reportContent = GeneratePayrollReport();
                break;
            case "Contratos":
                reportContent = GenerateContractsReport();
                break;
            default:
                reportContent = "Tipo de reporte no v�lido.";
                break;
        }

        ReportPreviewEditor.Text = reportContent;
    }

    // Generar reporte de evaluaciones de desempe�o
    private string GeneratePerformanceEvaluationReport()
    {
        var evaluations = _repository.EvaluacionesDesempeno
            .Select(e => $"Empleado: {e.Empleado.Nombre}, Puntuaci�n: {e.Puntuacion}, Fecha: {e.FechaEvaluacion.ToShortDateString()}")
            .ToList();

        return string.Join(Environment.NewLine, evaluations);
    }

    // Generar reporte de n�minas
    private string GeneratePayrollReport()
    {
        var payrolls = _repository.Nominas
            .Select(n => $"Empleado: {n.Empleado.Nombre}, Salario Neto: {n.SalarioNeto}, Fecha de Pago: {n.FechaPago.ToShortDateString()}")
            .ToList();

        return string.Join(Environment.NewLine, payrolls);
    }

    // Generar reporte de contratos
    private string GenerateContractsReport()
    {
        var contracts = _repository.Contratos
            .Select(c => $"Empleado: {c.Empleado.Nombre}, Tipo: {c.TipoContrato}, Desde: {c.FechaInicio.ToShortDateString()} Hasta: {c.FechaFin.ToShortDateString()}")
            .ToList();

        return string.Join(Environment.NewLine, contracts);
    }

    // Bot�n para regresar
    private async void OnBackButtonClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//EmployeeListPage");
    }
}
