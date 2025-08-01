
using Microsoft.EntityFrameworkCore;
using MinimalAPI.Domain.Entities;
using MinimalAPI.Domain.Interface;
using MinimalAPI.DTOs;
using MinimalAPI.Infrastructure;

namespace MinimalAPI.Domain.Services
{
    public class AdministratorService : IAdministratorService
    {

        private readonly DbContexto _contexto;
        public AdministratorService(DbContexto dbcontexto)
        {
            _contexto = dbcontexto;
        }

        public Administrator? BuscarPorId(int id)
        {
            return _contexto.Administrators.Where(v => v.Id == id).FirstOrDefault();
        }

        public void Incluir(Administrator administrator)
        {
            _contexto.Administrators.Add(administrator);
            _contexto.SaveChanges();
        }

        public Administrator? Login(LoginDTO loginDTO)
        {

            var adm = _contexto.Administrators.Where(a => a.Email == loginDTO.Email && a.Password == loginDTO.Password).FirstOrDefault();

            return adm;
        }

        public List<Administrator> Todos(int? pagina = 1, string? email = null, string? perfil = null)
        {
            var query = _contexto.Administrators.AsQueryable();

            if (!string.IsNullOrEmpty(email))
            {
                query = query.Where(a => EF.Functions.Like(a.Email.ToLower(), $"%{email.ToLower()}%"));
            }

            int itensPorPagina = 10;

            if (pagina != null)
                query = query.Skip(((int)pagina - 1) * itensPorPagina).Take(itensPorPagina);



            return query.ToList();
        }
        public void Atualizar(Administrator administrator)
        {
            _contexto.Administrators.Update(administrator);
            _contexto.SaveChanges();
        }

        public void Apagar(Administrator administrator)
        {
            _contexto.Administrators.Remove(administrator);
            _contexto.SaveChanges();
        }



    }
}