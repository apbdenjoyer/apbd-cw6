using System.ComponentModel.DataAnnotations;

namespace apbd_cw6.Models.DTOs;

public class AddAnimal
{
    [Required] [MaxLength(200)] public string Name { get; set; }
    [MaxLength(200)] public string? Description { get; set; }
    [Required] [MaxLength(200)] public string Category { get; set; }
    [Required] [MaxLength(200)] public string Area { get; set; }
}