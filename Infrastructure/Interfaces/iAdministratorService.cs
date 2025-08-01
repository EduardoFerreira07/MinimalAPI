using MinimalAPI.Domain.Entities;
using MinimalAPI.DTOs;

namespace MinimalAPI.Domain.Interface
{
    public interface IAdministratorService
    {

        Administrator? Login(LoginDTO loginDTO);

        void Incluir(Administrator administrator);

        List<Administrator> Todos(int? pagina = 1, string? email = null, string? perfil = null);

        public Administrator? BuscarPorId(int id);

        void Atualizar(Administrator administrator);

        void Apagar(Administrator administrator);


        



    }
}