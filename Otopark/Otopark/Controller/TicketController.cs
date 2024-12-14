using Microsoft.AspNetCore.Mvc;
using Otopark.DbContext;
using Otopark.Models;

namespace Otopark.Controller;

[ApiController]
[Route("api/[controller]")]
public class TicketController : ControllerBase
{
    public ApplicationDbContext _context;

    public TicketController(ApplicationDbContext context)
    {
        this._context = context;
    }

    // true =kullanılıyor doulu
    
    [HttpPost("{SpotId}/{CarId}")]
    public ActionResult<Ticket> CreateTicket(int SpotId, int CarId)
    {
        var spot = _context.Spots.Find(SpotId);
        if (spot == null)
        {
            return BadRequest("Yer bulunamadı");
        }

        // Eğer spot doluysa (Status == true), işlem yapılmaz
        if (spot.Status)
        {
            return BadRequest("Yer uygun değil");
        }
        
        
        var car = _context.Cars.Find(CarId);
        if (car == null)
        {
            return BadRequest("Araba bulunamadı");
        }
        
        var ticket = new Ticket()
        {
            SpotId = SpotId,
            CarId = CarId,
            EntryTime = DateTime.UtcNow,
            Status = "true"  // Bilet durumu "true" olarak ayarlanır  true =kullanılıyor doulu anlamına geliyor.
        };

        // Spot durumu "dolu" olarak güncellenir
        spot.Status = true;
        
        _context.Tickets.Add(ticket);
        _context.Spots.Update(spot);
        _context.SaveChanges();

        return Ok(ticket);

    }
    

    }
    