using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using minimal_api.Domain.Services;
using minimal_api.Infrastructure.Interfaces;
using MinimalAPI.Domain.Entities;
using MinimalAPI.Domain.Enuns;
using MinimalAPI.Domain.Interface;
using MinimalAPI.Domain.ModelViews;
using MinimalAPI.Domain.Services;
using MinimalAPI.DTOs;
using MinimalAPI.Infrastructure;

#region Builder
var builder = WebApplication.CreateBuilder(args);

var key = builder.Configuration.GetSection("Jwt").ToString();
if (string.IsNullOrEmpty(key)) key = "123456";

builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(option =>
{
    option.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddAuthorization();

builder.Services.AddScoped<IAdministratorService, AdministratorService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira o Token JWT aqui"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
{
    {
        new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            }
        },
        new string[] { }
    }
});

});

builder.Services.AddDbContext<DbContexto>
(
    options =>
    {
        options.UseMySql(
            builder.Configuration.GetConnectionString("mysql"),
            ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mysql"))
        );
    }
);

var app = builder.Build();
#endregion

#region Home
app.MapGet("/", () => Results.Json(new Home())).AllowAnonymous().WithTags("Home");
#endregion

#region Administrators

string GerarTokenJwt(Administrator administrator)
{
    if (string.IsNullOrEmpty(key)) return string.Empty;

    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);


    var claims = new List<Claim>()
    {

        new Claim("Email", administrator.Email),
        new Claim("Perfil", administrator.Perfil),
        new Claim(ClaimTypes.Role, administrator.Perfil),


    };


    var token = new JwtSecurityToken(
        claims: claims,
        expires: DateTime.Now.AddDays(1),
        signingCredentials: credentials

    );

    return new JwtSecurityTokenHandler().WriteToken(token);
}

app.MapPost("/administrator/login", ([FromBody] LoginDTO loginDTO, IAdministratorService administratorService) =>
{
    var adm = administratorService.Login(loginDTO);
    if (adm != null)
    {
        string token = GerarTokenJwt(adm);
        return Results.Ok(new AdmLogged { Email = adm.Email, Perfil = adm.Perfil, Token = token });
    }
    else
    {
        return Results.Unauthorized();
    }
}).AllowAnonymous().WithTags("Administrators");


app.MapPost("/administrator", ([FromBody] AdministratorDTO administratorDTO, IAdministratorService administratorService) =>
{
    var validacao = new ErroDeValidacao
    {
        Mensagens = new List<string>()
    };

    if (string.IsNullOrEmpty(administratorDTO.Email))
    {
        validacao.Mensagens.Add("Email não pode ser vazio!");
    }
    if (string.IsNullOrEmpty(administratorDTO.Password))
    {
        validacao.Mensagens.Add("Senha não pode ser vazia!");
    }
    if (!Enum.IsDefined(typeof(Perfil), administratorDTO.Perfil))
        validacao.Mensagens.Add("Perfil inválido");


    if (validacao.Mensagens.Count > 0)
        return Results.BadRequest(validacao);


    var administrator = new Administrator
    {
        Email = administratorDTO.Email,
        Password = administratorDTO.Password,
        Perfil = administratorDTO.Perfil.ToString()
    };

    administratorService.Incluir(administrator);
    return Results.Created($"/administrator/{administrator.Id}", administrator);



}).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "adm" }).WithTags("Administrators");

app.MapGet("/administrators", ([FromQuery] int? pagina, IAdministratorService administratorService) =>
{
    var admins = administratorService.Todos(pagina);
    var adminsModel = AdministratorModelViewFactory.FromEntityList(admins);
    return Results.Ok(adminsModel);

}).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "adm" }).WithTags("Administrators");

app.MapGet("/administrators/{id}", ([FromRoute] int id, IAdministratorService administratorService) =>
{
    var admin = administratorService.BuscarPorId(id);
    if (admin == null) return Results.NotFound();

    var adminModel = AdministratorModelViewFactory.FromEntity(admin);
    return Results.Ok(adminModel);
}).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "adm" }).WithTags("Administrators");


app.MapPut("/administrators/{id}", (
    [FromRoute] int id,
    [FromBody] AdministratorDTO administratorDTO,
    IAdministratorService administratorService) =>
{
    var validacao = new ErroDeValidacao { Mensagens = new List<string>() };

    if (string.IsNullOrEmpty(administratorDTO.Email))
        validacao.Mensagens.Add("Email não pode ser vazio!");
    if (string.IsNullOrEmpty(administratorDTO.Password))
        validacao.Mensagens.Add("Senha não pode ser vazia!");
    if (!Enum.IsDefined(typeof(Perfil), administratorDTO.Perfil))
        validacao.Mensagens.Add("Perfil inválido");

    if (validacao.Mensagens.Count > 0)
        return Results.BadRequest(validacao);

    var existing = administratorService.BuscarPorId(id);
    if (existing == null)
        return Results.NotFound();

    existing.Email = administratorDTO.Email;
    existing.Password = administratorDTO.Password;
    existing.Perfil = administratorDTO.Perfil.ToString();

    administratorService.Atualizar(existing);

    return Results.Ok(existing);
}).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "adm" }).WithTags("Administrators");

app.MapDelete("/administrators/{id}", (
    [FromRoute] int id,
    IAdministratorService administratorService) =>
{
    var admin = administratorService.BuscarPorId(id);
    if (admin == null)
        return Results.NotFound();

    administratorService.Apagar(admin);
    return Results.NoContent();
}).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "adm" }).WithTags("Administrators");


#endregion

#region Vehicles

ErroDeValidacao validaDTO(VehicleDTO vehicleDTO)
{
    var validacao = new ErroDeValidacao
    {
        Mensagens = new List<string>()
    };

    if (string.IsNullOrEmpty(vehicleDTO.Nome))
    {
        validacao.Mensagens.Add("Nome não pode ser nulo!");
    }
    if (string.IsNullOrEmpty(vehicleDTO.Marca))
    {
        validacao.Mensagens.Add("Marca não pode ser nulo!");
    }
    if (vehicleDTO.Ano < 1950)
    {
        validacao.Mensagens.Add("Veículo muito antigo!");
    }

    return validacao;

}

app.MapPost("/vehicle", ([FromBody] VehicleDTO vehicleDTO, IVehicleService vehicleService) =>
{
    var validacao = validaDTO(vehicleDTO);
    if (validacao.Mensagens.Count > 0)
        return Results.BadRequest(validacao);

    var vehicle = new Vehicle
    {
        Nome = vehicleDTO.Nome,
        Marca = vehicleDTO.Marca,
        Ano = vehicleDTO.Ano
    };
    vehicleService.Incluir(vehicle);

    return Results.Created($"/vehicle/{vehicle.Id}", vehicle);
}).RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute { Roles = "adm" })
.RequireAuthorization(new AuthorizeAttribute { Roles = "editor" })
.WithTags("Vehicles");

app.MapGet("/vehicle", ([FromQuery] int? pagina, IVehicleService vehicleService) =>
{
    var vehicles = vehicleService.Todos(pagina);

    return Results.Ok(vehicles);
}).RequireAuthorization().WithTags("Vehicles");

app.MapGet("/vehicle/{id}", ([FromRoute] int id, IVehicleService vehicleService) =>
{
    var vehicle = vehicleService.BuscaPorId(id);
    if (vehicle == null) return Results.NotFound();
    return Results.Ok(vehicle);
}).RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute { Roles = "adm" })
.RequireAuthorization(new AuthorizeAttribute { Roles = "editor" })
.WithTags("Vehicles");

app.MapPut("/vehicle/{id}", ([FromRoute] int id, VehicleDTO vehicleDTO, IVehicleService vehicleService) =>
{
    var vehicle = vehicleService.BuscaPorId(id);
    if (vehicle == null) return Results.NotFound();

    var validacao = validaDTO(vehicleDTO);
    if (validacao.Mensagens.Count > 0)
        return Results.BadRequest(validacao);


    vehicle.Nome = vehicleDTO.Nome;
    vehicle.Marca = vehicleDTO.Marca;
    vehicle.Ano = vehicleDTO.Ano;

    vehicleService.Atualizar(vehicle);

    return Results.Ok(vehicle);
}).RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute { Roles = "adm" })
.WithTags("Vehicles");

app.MapDelete("/vehicle/{id}", ([FromRoute] int id, IVehicleService vehicleService) =>
{
    var vehicle = vehicleService.BuscaPorId(id);
    if (vehicle == null)
    {
        return Results.NotFound();
    }
    else
    {
        vehicleService.Apagar(vehicle);
        return Results.NoContent();
    }

}).RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute { Roles = "adm" })
.WithTags("Vehicles");

#endregion

#region App
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.Run();
#endregion



