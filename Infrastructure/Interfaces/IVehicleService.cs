using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MinimalAPI.Domain.Entities;

namespace minimal_api.Infrastructure.Interfaces
{
    public interface IVehicleService
    {
        List<Vehicle> Todos(int? pagina = 1, string? nome = null, string? marca = null);

        Vehicle? BuscaPorId(int id);

        void Incluir(Vehicle vehicle);

        void Atualizar(Vehicle vehicle);

        void Apagar(Vehicle vehicle);
    }
}