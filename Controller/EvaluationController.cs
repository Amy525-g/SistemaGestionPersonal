using SistemaGestionPersonal.Data;
using SistemaGestionPersonal.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SistemaGestionPersonal.Controller
{
    public class EvaluationController
    {
        private readonly InMemoryRepository _repository;

        public EvaluationController(InMemoryRepository repository)
        {
            _repository = repository;
        }

        // Agregar una nueva evaluación de desempeño
        public void AddEvaluation(int empleadoId, DateTime fechaEvaluacion, int puntuacion, string comentarios)
        {
            if (puntuacion < 1 || puntuacion > 10)
                throw new Exception("La puntuación debe estar entre 1 y 10.");

            var empleado = _repository.Empleados.FirstOrDefault(e => e.IdEmpleado == empleadoId);
            if (empleado == null)
                throw new Exception("Empleado no encontrado.");

            var evaluacion = new EvaluacionDesempeno
            {
                IdEvaluacion = _repository.EvaluacionesDesempeno.Count + 1,
                IdEmpleado = empleadoId,
                FechaEvaluacion = fechaEvaluacion,
                Puntuacion = puntuacion,
                Comentarios = comentarios,
                Empleado = empleado
            };

            _repository.EvaluacionesDesempeno.Add(evaluacion);

        }

        // Obtener todas las evaluaciones de desempeño
        public List<EvaluacionDesempeno> GetAllEvaluations()
        {
            return _repository.EvaluacionesDesempeno.ToList();
        }

        // Actualizar una evaluación de desempeño
        public void UpdateEvaluation(int idEvaluacion, DateTime fechaEvaluacion, int puntuacion, string comentarios)
        {
            var evaluacion = _repository.EvaluacionesDesempeno.FirstOrDefault(e => e.IdEvaluacion == idEvaluacion);
            if (evaluacion == null)
                throw new Exception("Evaluación no encontrada.");

            if (puntuacion < 1 || puntuacion > 10)
                throw new Exception("La puntuación debe estar entre 1 y 10.");

            evaluacion.FechaEvaluacion = fechaEvaluacion;
            evaluacion.Puntuacion = puntuacion;
            evaluacion.Comentarios = comentarios;

        }

        // Eliminar una evaluación de desempeño
        public void DeleteEvaluation(int idEvaluacion)
        {
            var evaluacion = _repository.EvaluacionesDesempeno.FirstOrDefault(e => e.IdEvaluacion == idEvaluacion);
            if (evaluacion == null)
                throw new Exception("Evaluación no encontrada.");

            _repository.EvaluacionesDesempeno.Remove(evaluacion);

        }
    }
}
