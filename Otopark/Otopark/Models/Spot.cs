using System.Text.Json.Serialization;

namespace Otopark.Models;

public class Spot
{
    
    public int Id { get; set; } 
    
    public string Code { get; set; }
    
    public bool Status { get; set; }
    public int TypeId { get; set; }
    
    public VehicleType VehicleType { get; set; }
    [JsonIgnore]
    public ICollection<Ticket> Tickets { get; set; }
}
