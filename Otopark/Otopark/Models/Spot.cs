namespace Otopark.Models;

public class Spot
{
    public int Id { get; set; } 
    
    public string Code { get; set; }
    
    public string Status { get; set; }
    public int TypeId { get; set; }
    
   
    public VehicleType VehicleType { get; set; }
    public ICollection<Ticket> Tickets { get; set; }
}
