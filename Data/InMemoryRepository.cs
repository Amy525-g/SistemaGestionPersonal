using System;
using System.Collections.Generic;
using SistemaGestionPersonal.Models;
using System.IO;
using System.Text.Json;

namespace SistemaGestionPersonal.Data
{
    public class InMemoryRepository
    {
        // Listas simuladas para almacenar los datos
        public List<User> Users { get; set; } = new List<User>();
        public List<Role> Roles { get; set; } = new List<Role>();
        public List<Empleado> Empleados { get; set; } = new List<Empleado>();
        public List<Contrato> Contratos { get; set; } = new List<Contrato>();
        public List<Nomina> Nominas { get; set; } = new List<Nomina>();
        public List<EvaluacionDesempeno> EvaluacionesDesempeno { get; set; } = new List<EvaluacionDesempeno>();
        public List<Bono> Bonos { get; set; } = new List<Bono>();

        public InMemoryRepository()
        {
            InitializeDefaultData();
        }

        // Método para inicializar datos por defecto
        private void InitializeDefaultData()
        {
            // Agregar roles
            var adminRole = new Role { RoleID = 1, RoleName = "Admin" };
            var employeeRole = new Role { RoleID = 2, RoleName = "Employee" };
            Roles.AddRange(new[] { adminRole, employeeRole });

            // Agregar usuarios
            Users.Add(new User { UserID = 1, Username = "admin", PasswordHash = "1234", Role = adminRole });
            Users.Add(new User { UserID = 2, Username = "John", PasswordHash = "1234", Role = employeeRole });
            Users.Add(new User { UserID = 3, Username = "Jane", PasswordHash = "5678", Role = employeeRole });
            Users.Add(new User { UserID = 4, Username = "Amy Cherrez", PasswordHash = "0000", Role = adminRole });


            // Agregar empleados
            Empleados.Add(new Empleado { IdEmpleado = 1, Nombre = "John", Apellido = "Doe", FechaNacimiento = new DateTime(1990, 1, 1), Correo = "john.doe@example.com", UserID = 2 });
            Empleados.Add(new Empleado { IdEmpleado = 2, Nombre = "Jane", Apellido = "Smith", FechaNacimiento = new DateTime(2000, 5, 15), Correo = "jane.smith@example.com", UserID = 3 });

            // Agregar contratos
            Contratos.Add(new Contrato { IdContrato = 1, IdEmpleado = 1, FechaInicio = DateTime.Now.AddMonths(-6), FechaFin = DateTime.Now.AddMonths(6), TipoContrato = "Fijo", Empleado = Empleados[0] });
            Contratos.Add(new Contrato { IdContrato = 2, IdEmpleado = 2, FechaInicio = DateTime.Now.AddMonths(-3), FechaFin = DateTime.Now.AddMonths(9), TipoContrato = "Temporal", Empleado = Empleados[1] });

            // Agregar nóminas
            Nominas.Add(new Nomina { IdNomina = 1, IdEmpleado = 1, SalarioBruto = 2000, Deducciones = 200, SalarioNeto = 1800, FechaPago = DateTime.Now.AddMonths(-1), Empleado = Empleados[0] });
            Nominas.Add(new Nomina { IdNomina = 2, IdEmpleado = 2, SalarioBruto = 2500, Deducciones = 300, SalarioNeto = 2200, FechaPago = DateTime.Now.AddMonths(-1), Empleado = Empleados[1] });

            // Agregar nóminas para el empleado con IdEmpleado = 1
            Nominas.Add(new Nomina
            {
                IdNomina = 1,
                IdEmpleado = 1,
                SalarioBruto = 2000,
                Deducciones = 150,
                SalarioNeto = 1850,
                FechaPago = DateTime.Now.AddMonths(-3),
                Empleado = Empleados.First(e => e.IdEmpleado == 1)
            });

            Nominas.Add(new Nomina
            {
                IdNomina = 2,
                IdEmpleado = 1,
                SalarioBruto = 2000,
                Deducciones = 200,
                SalarioNeto = 1800,
                FechaPago = DateTime.Now.AddMonths(-2),
                Empleado = Empleados.First(e => e.IdEmpleado == 1)
            });

            Nominas.Add(new Nomina
            {
                IdNomina = 3,
                IdEmpleado = 1,
                SalarioBruto = 2000,
                Deducciones = 250,
                SalarioNeto = 1750,
                FechaPago = DateTime.Now.AddMonths(-1),
                Empleado = Empleados.First(e => e.IdEmpleado == 1)
            });

            // Agregar evaluaciones para el empleado con IdEmpleado = 1
            EvaluacionesDesempeno.Add(new EvaluacionDesempeno
            {
                IdEvaluacion = 1,
                IdEmpleado = 1,
                FechaEvaluacion = DateTime.Now.AddMonths(-2),
                Puntuacion = 8,
                Comentarios = "Buen desempeño general",
                Empleado = Empleados.First(e => e.IdEmpleado == 1)
            });

            EvaluacionesDesempeno.Add(new EvaluacionDesempeno
            {
                IdEvaluacion = 2,
                IdEmpleado = 1,
                FechaEvaluacion = DateTime.Now.AddMonths(-1),
                Puntuacion = 9,
                Comentarios = "Gran capacidad de trabajo en equipo",
                Empleado = Empleados.First(e => e.IdEmpleado == 1)
            });

            Bonos.Add(new Bono
            {
                IdBono = 1,
                IdEmpleado = 1,
                Categoria = "Sobresaliente",
                Porcentaje = 20,
                FechaAsignacion = DateTime.Now,
                MontoTotal = 400, // Ejemplo: salario * porcentaje
                Empleado = Empleados.FirstOrDefault(e => e.IdEmpleado == 1)
            });

            Bonos.Add(new Bono
            {
                IdBono = 2,
                IdEmpleado = 2,
                Categoria = "Bueno",
                Porcentaje = 10,
                FechaAsignacion = DateTime.Now,
                MontoTotal = 250,
                Empleado = Empleados.FirstOrDefault(e => e.IdEmpleado == 2)
            });
            Empleados.Add(new Empleado { IdEmpleado = 1, Nombre = "Juan", Apellido = "Perez" });
            EvaluacionesDesempeno.Add(new EvaluacionDesempeno { IdEvaluacion = 1, IdEmpleado = 1, Puntuacion = 9, FechaEvaluacion = DateTime.Now.AddMonths(-1) });
            Contratos.Add(new Contrato { IdContrato = 1, IdEmpleado = 1, FechaInicio = DateTime.Now.AddMonths(-3), FechaFin = DateTime.Now.AddMonths(9) });
            Nominas.Add(new Nomina { IdNomina = 1, IdEmpleado = 1, SalarioBruto = 2000, Deducciones = 200 });
        }
    }
}
