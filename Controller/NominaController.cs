using SistemaGestionPersonal.Data;
using SistemaGestionPersonal.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SistemaGestionPersonal.Controller
{
    public class NominaController
    {
        private readonly InMemoryRepository _repository;

        public NominaController(InMemoryRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public void AddNomina(int empleadoId, decimal salarioBruto, decimal deducciones, DateTime fechaPago)
        {
            if (salarioBruto < deducciones)
            {
                throw new ArgumentException("Las deducciones no pueden superar el salario bruto.");
            }

            var empleado = _repository.Empleados.FirstOrDefault(e => e.IdEmpleado == empleadoId);
            if (empleado == null)
            {
                throw new KeyNotFoundException("Empleado no encontrado.");
            }

            var nomina = new Nomina
            {
                IdNomina = _repository.Nominas.Count + 1,
                IdEmpleado = empleadoId,
                SalarioBruto = salarioBruto,
                Deducciones = deducciones,
                SalarioNeto = salarioBruto - deducciones,
                FechaPago = fechaPago,
                Empleado = empleado
            };

            _repository.Nominas.Add(nomina);

        }

        public List<Nomina> GetAllNominas()
        {
            return _repository.Nominas;
        }

        public Nomina GetNominaById(int nominaId)
        {
            return _repository.Nominas.FirstOrDefault(n => n.IdNomina == nominaId)
                   ?? throw new KeyNotFoundException("Nómina no encontrada.");
        }

        public void UpdateNomina(int nominaId, decimal salarioBruto, decimal deducciones, DateTime fechaPago)
        {
            var nomina = _repository.Nominas.FirstOrDefault(n => n.IdNomina == nominaId);
            if (nomina == null)
            {
                throw new KeyNotFoundException("Nómina no encontrada.");
            }

            if (salarioBruto < deducciones)
            {
                throw new ArgumentException("Las deducciones no pueden superar el salario bruto.");
            }

            nomina.SalarioBruto = salarioBruto;
            nomina.Deducciones = deducciones;
            nomina.SalarioNeto = salarioBruto - deducciones;
            nomina.FechaPago = fechaPago;

        }

        public void DeleteNomina(int nominaId)
        {
            var nomina = _repository.Nominas.FirstOrDefault(n => n.IdNomina == nominaId);
            if (nomina == null)
            {
                throw new KeyNotFoundException("Nómina no encontrada.");
            }

            _repository.Nominas.Remove(nomina);
        }
    }
}
