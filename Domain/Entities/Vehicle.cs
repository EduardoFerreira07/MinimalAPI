

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinimalAPI.Domain.Entities;

    public class Vehicle
    {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; } = default!;

    [Required]
    [StringLength(150)]
    public string Nome { get; set; } = default!;


    [StringLength(50)]
    public string Marca { get; set; } = default!;


    public int Ano { get; set; } = default!;

    }
