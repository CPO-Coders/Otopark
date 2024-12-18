using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Otopark.DbContext;
using Otopark.DTO;
using Otopark.Models;

namespace Otopark.Controller;

[ApiController]
[Route("api/[controller]")]
public class SpotController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public SpotController(ApplicationDbContext context)
    {
        _context = context;
    }

    
    [HttpGet]
    public async Task<IActionResult> GetAllSpots()
    {
        var spots = await _context.Spots
            .Include(s => s.VehicleType) // İlişkileri dahil et
            .ToListAsync();
        return Ok(spots);
    }

    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetSpotById(int id)
    {
        var spot = await _context.Spots
            .Include(s => s.VehicleType)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (spot == null)
            return NotFound("Spot bulunamadı.");

        return Ok(spot);
    }

    
    [HttpPost]
    public async Task<IActionResult> CreateSpot([FromBody] SpotDto spotDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var isCodeExist = await _context.Spots.FirstOrDefaultAsync(s => s.Code == spotDto.Code);
        if (isCodeExist != null)
        {
            return BadRequest($"Code {spotDto.Code} already exists.");
        }
         
        var vehicleTypeExists = await _context.VehicleTypes.AnyAsync(v => v.Id == spotDto.TypeId);
        if (!vehicleTypeExists)
            return BadRequest($"Geçersiz TypeId: {spotDto.TypeId}. Bu ID'ye sahip bir VehicleType yok.");

        var spot = new Spot
        {
            Code = spotDto.Code,
            Status = spotDto.Status,
            TypeId = spotDto.TypeId
        };

        _context.Spots.Add(spot);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetSpotById), new { id = spot.Id }, spot);
    }

    
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSpot(int id, [FromBody] SpotDto spotDto)
    {
        var spot = await _context.Spots.FindAsync(id);

        if (spot == null)
            return NotFound("Spot bulunamadı.");

        var vehicleTypeExists = await _context.VehicleTypes.AnyAsync(v => v.Id == spotDto.TypeId);
        if (!vehicleTypeExists)
            return BadRequest($"Geçersiz TypeId: {spotDto.TypeId}. Bu ID'ye sahip bir VehicleType yok.");

        spot.Code = spotDto.Code;
        spot.Status = spotDto.Status;
        spot.TypeId = spotDto.TypeId;

        _context.Spots.Update(spot);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSpot(int id)
    {
        var spot = await _context.Spots.FindAsync(id);

        if (spot == null)
            return NotFound("Spot bulunamadı.");

        _context.Spots.Remove(spot);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    
    [HttpPut("ParkActive: {id}")]
    public async Task<IActionResult> SpotActive(int id)
    {
        var spot = await _context.Spots.FindAsync(id);

        if (spot == null)
            return NotFound("Spot bulunamadı.");
        if (spot.Status)
            return BadRequest("Bu yer zaten dolu.");

        spot.Status = true; // Yer dolu yapılıyor
        _context.Spots.Update(spot);

        await _context.SaveChangesAsync();
        return Ok($"Yer '{spot.Code}' başarıyla dolu olarak işaretlendi.");
    }

    
    [HttpPut("ParkPassive{id}")]
    public async Task<IActionResult> SpotPassive(int id)
    {
        var spot = await _context.Spots.FindAsync(id);

        if (spot == null)
            return NotFound("Spot bulunamadı.");
        if (!spot.Status)
            return BadRequest("Bu yer zaten boş.");

        spot.Status = false; // Yer boş yapılıyor
        _context.Spots.Update(spot);

        await _context.SaveChangesAsync();
        return Ok($"Yer '{spot.Code}' başarıyla boş olarak işaretlendi.");
    }
}
