using System.ComponentModel.DataAnnotations;

namespace Otopark.DTO;

public class SpotDto
{
    [Required]
    public string Code { get; set; }
    [Required]
    public bool Status { get; set; }
    [Required]
    public int TypeId { get; set; }
}