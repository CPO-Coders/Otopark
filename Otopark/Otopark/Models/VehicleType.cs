namespace Otopark.Models;

public class VehicleType
{
    public int Id { get; set; } 
    public string Name { get; set; }
    public ICollection<Car> Cars { get; set; }
}


