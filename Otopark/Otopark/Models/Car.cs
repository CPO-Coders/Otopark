namespace Otopark.Models;

public class Car
{
    public int Id { get; set; } 
    
    public string Plate { get; set; } 
    
    public string? mark { get; set; }
    public int TypeId { get; set; } 
    public int UserId { get; set; } 
    public User? User { get; set; }
    public VehicleType? VehicleType { get; set; }
}
