using Microsoft.EntityFrameworkCore;
using MinimalAPI.Domain.Entities;

namespace MinimalAPI.Infrastructure;


public class DbContexto : DbContext
{

    private readonly IConfiguration _configuracaoAppSettings;
    public DbContexto(IConfiguration configuracaoAppSettings )
    {
        _configuracaoAppSettings = configuracaoAppSettings;
    }

    public DbSet<Administrator> Administrators { get; set; } = default!;

    public DbSet<Vehicle> Vehicles { get; set; } = default!;


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Administrator>().HasData
        (
            new Administrator
            {
                Id = 1,
                Email = "administrator@test.com",
                Password = "abc123",
                Perfil = "Adm"
            }
        );
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var stringConexao = _configuracaoAppSettings.GetConnectionString("mysql")?.ToString();
            if (!string.IsNullOrEmpty(stringConexao))
            {
                optionsBuilder.UseMySql
            (
                stringConexao,
                ServerVersion.AutoDetect(stringConexao)
            );
            }
        }


    }
}