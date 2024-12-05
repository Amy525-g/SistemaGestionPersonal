using SistemaGestionPersonal.Data;
using SistemaGestionPersonal.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SistemaGestionPersonal.Controller
{
    public class ContractController
    {
        private readonly InMemoryRepository _repository;

        public ContractController(InMemoryRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public void AddContract(int employeeId, DateTime startDate, DateTime endDate, string contractType)
        {
            if (startDate >= endDate)
                throw new ArgumentException("La fecha de inicio debe ser anterior a la fecha de fin.");

            var employee = _repository.Empleados.FirstOrDefault(e => e.IdEmpleado == employeeId);
            if (employee == null)
                throw new KeyNotFoundException("Empleado no encontrado.");

            var contract = new Contrato
            {
                IdContrato = _repository.Contratos.Count + 1,
                IdEmpleado = employeeId,
                FechaInicio = startDate,
                FechaFin = endDate,
                TipoContrato = contractType,
                Empleado = employee
            };

            _repository.Contratos.Add(contract);
        }

        public List<Contrato> GetAllContracts()
        {
            return _repository.Contratos;
        }

        public Contrato GetContractById(int contractId)
        {
            return _repository.Contratos.FirstOrDefault(c => c.IdContrato == contractId)
                   ?? throw new KeyNotFoundException("Contrato no encontrado.");
        }

        public void UpdateContract(int contractId, int employeeId, DateTime startDate, DateTime endDate, string contractType)
        {
            var contract = _repository.Contratos.FirstOrDefault(c => c.IdContrato == contractId);
            if (contract == null)
                throw new KeyNotFoundException("Contrato no encontrado.");

            var employee = _repository.Empleados.FirstOrDefault(e => e.IdEmpleado == employeeId);
            if (employee == null)
                throw new KeyNotFoundException("Empleado no encontrado.");

            if (startDate >= endDate)
                throw new ArgumentException("La fecha de inicio debe ser anterior a la fecha de fin.");

            contract.IdEmpleado = employeeId;
            contract.FechaInicio = startDate;
            contract.FechaFin = endDate;
            contract.TipoContrato = contractType;

        }

        public void DeleteContract(int contractId)
        {
            var contract = _repository.Contratos.FirstOrDefault(c => c.IdContrato == contractId);
            if (contract == null)
                throw new KeyNotFoundException("Contrato no encontrado.");

            _repository.Contratos.Remove(contract);
        }
    }
}
