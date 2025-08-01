using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MinimalAPI.Domain.Enuns;

namespace MinimalAPI.DTOs;

    public class AdministratorDTO
    {


    public string Email { get; set; } = default!;

    public string Password { get; set; } = default!;

    public Perfil Perfil { get; set; } = default!;




    }
