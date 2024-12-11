namespace Otopark.Models;

public class Ticket
{
    public int Id { get; set; } 
    public int CarId { get; set; }
    public int SpotId { get; set; }
    
    public DateTime EntryTime { get; set; }
    
    public DateTime? ExitTime { get; set; } 
    
    public decimal? Fee { get; set; } 
    
    public string Status { get; set; }
    
    public Car Car { get; set; }
    public Spot Spot { get; set; }
}
