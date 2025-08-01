using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using minimal_api.Infrastructure.Interfaces;
using MinimalAPI.Domain.Entities;
using MinimalAPI.Infrastructure;

namespace minimal_api.Domain.Services
{
    public class VehicleService : IVehicleService
    {

        private readonly DbContexto _contexto;
        public VehicleService(DbContexto dbcontexto)
        {
            _contexto = dbcontexto;
        }
        public void Apagar(Vehicle vehicle)
        {
            _contexto.Vehicles.Remove(vehicle);
            _contexto.SaveChanges();
        }

        public void Atualizar(Vehicle vehicle)
        {
            _contexto.Vehicles.Update(vehicle);
            _contexto.SaveChanges();
        }

        public Vehicle? BuscaPorId(int id)
        {
            return _contexto.Vehicles.Where(v => v.Id == id).FirstOrDefault();
        }

        public void Incluir(Vehicle vehicle)
        {
            _contexto.Vehicles.Add(vehicle);
            _contexto.SaveChanges();
        }

        public List<Vehicle> Todos(int? pagina = 1, string? nome = null, string? marca = null)
        {
            var query = _contexto.Vehicles.AsQueryable();

            if (!string.IsNullOrEmpty(nome))
            {
                query = query.Where(v => EF.Functions.Like(v.Nome.ToLower(), $"%{nome.ToLower()}%"));
            }

            int itensPorPagina = 10;

            if (pagina != null) 
                query = query.Skip(((int)pagina - 1) * itensPorPagina).Take(itensPorPagina);
            
            

            return query.ToList();
        }
    }
}