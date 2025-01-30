using SistemaGestionPersonal.Data;
using SistemaGestionPersonal.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SistemaGestionPersonal.Views;

public partial class PayrollPage : ContentPage
{
    private readonly MySqlConnectionProvider _connectionProvider;
    private readonly int _currentEmployeeId;

    public PayrollPage(int employeeId)
    {
        InitializeComponent();
        _connectionProvider = new MySqlConnectionProvider();
        _currentEmployeeId = employeeId;

        LoadPayroll();
    }

    // Cargar n�minas del empleado actual
    private void LoadPayroll()
    {
        try
        {
            using var connection = _connectionProvider.GetConnection();
            connection.Open();

            string query = @"
                SELECT FechaPago, SalarioBruto, Deducciones, SalarioNeto
                FROM Nomina
                WHERE IdEmpleado = @IdEmpleado
                ORDER BY FechaPago DESC";

            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@IdEmpleado", _currentEmployeeId);

            using var reader = command.ExecuteReader();

            var payrolls = new List<Nomina>();

            while (reader.Read())
            {
                payrolls.Add(new Nomina
                {
                    FechaPago = reader.GetDateTime("FechaPago"),
                    SalarioBruto = reader.GetDecimal("SalarioBruto"),
                    Deducciones = reader.GetDecimal("Deducciones"),
                    SalarioNeto = reader.GetDecimal("SalarioNeto")
                });
            }

            if (!payrolls.Any())
            {
                DisplayAlert("Informaci�n", "No se encontraron n�minas para el empleado actual.", "OK");
            }

            PayrollListView.ItemsSource = payrolls;
        }
        catch (MySqlException ex)
        {
            DisplayAlert("Error", $"Error al cargar n�minas: {ex.Message}", "OK");
        }
        catch (Exception ex)
        {
            DisplayAlert("Error", $"Ocurri� un error inesperado: {ex.Message}", "OK");
        }
    }

    // Mostrar detalles de la n�mina seleccionada
    private async void OnPayrollSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem is Nomina selectedPayroll)
        {
            await DisplayAlert("Detalles de N�mina",
                $"Fecha de Pago: {selectedPayroll.FechaPago:dd/MM/yyyy}\n" +
                $"Salario Bruto: {selectedPayroll.SalarioBruto:C}\n" +
                $"Deducciones: {selectedPayroll.Deducciones:C}\n" +
                $"Salario Neto: {selectedPayroll.SalarioNeto:C}",
                "OK");
        }
    }

    // Bot�n para regresar
    private async void OnBackButtonClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//EmployeeHomePage");
    }
}
