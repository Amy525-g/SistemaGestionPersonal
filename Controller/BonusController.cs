using SistemaGestionPersonal.Data;
using SistemaGestionPersonal.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SistemaGestionPersonal.Controller
{
    public class BonusController
    {
        private readonly InMemoryRepository _repository;

        public BonusController(InMemoryRepository repository)
        {
            _repository = repository;
        }

        private Dictionary<int, int> GetLatestEvaluationScores()
        {
            return _repository.EvaluacionesDesempeno
                .GroupBy(e => e.IdEmpleado)
                .ToDictionary(
                    g => g.Key,
                    g => g.OrderByDescending(e => e.FechaEvaluacion).First().Puntuacion
                );
        }

        private (string categoria, decimal porcentaje) GetBonusCategoryAndPercentage(int puntuacion)
        {
            if (puntuacion >= 9)
                return ("Super", 30);
            if (puntuacion >= 8)
                return ("Sobresaliente", 20);
            if (puntuacion >= 6)
                return ("Bueno", 10);

            return ("No aplica", 0);
        }

        // Calcular y asignar bonos basados en las evaluaciones y contratos
        public void CalculateAndAssignBonuses()
        {
            var evaluationScores = GetLatestEvaluationScores();

            foreach (var empleado in _repository.Empleados)
            {
                if (!evaluationScores.ContainsKey(empleado.IdEmpleado))
                {
                    continue; 
                }

                var puntuacion = evaluationScores[empleado.IdEmpleado];
                var contrato = _repository.Contratos.FirstOrDefault(c => c.IdEmpleado == empleado.IdEmpleado);
                if (contrato == null)
                {
                    continue; 
                }

                var nomina = _repository.Nominas.FirstOrDefault(n => n.IdEmpleado == empleado.IdEmpleado);
                if (nomina == null || nomina.SalarioNeto <= 0)
                {
                    continue; 
                }

                var (categoria, porcentaje) = GetBonusCategoryAndPercentage(puntuacion);
                if (porcentaje == 0)
                {
                    continue; 
                }

                var montoTotal = nomina.SalarioNeto * porcentaje / 100;

                var bono = new Bono
                {
                    IdBono = _repository.Bonos.Count + 1,
                    IdEmpleado = empleado.IdEmpleado,
                    Categoria = categoria,
                    Porcentaje = porcentaje,
                    FechaAsignacion = DateTime.Now,
                    MontoTotal = montoTotal,
                    Empleado = empleado
                };

                _repository.Bonos.Add(bono);
            }
        }

        public List<Bono> GetBonuses()
        {
            return _repository.Bonos.ToList();
        }
    }
}
