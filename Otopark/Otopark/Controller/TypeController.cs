using Microsoft.AspNetCore.Mvc;
using Otopark.DbContext;
using Otopark.DTOs;
using Otopark.Models;
using Microsoft.EntityFrameworkCore;

namespace Otopark.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TypeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TypeController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<VehicleType>> AddType(VehicleTypeDto vehicleTypeDto)
        {
            // Eğer aynı isimde bir Type varsa tekrar eklememek için kontrol edelim
            var existingType = await _context.VehicleTypes
                .FirstOrDefaultAsync(vt => vt.Name == vehicleTypeDto.Name);

            if (existingType != null)
            {
                return BadRequest("Bu isimde bir araç tipi zaten mevcut.");
            }

            // Yeni bir VehicleType oluştur
            var vehicleType = new VehicleType
            {
                Name = vehicleTypeDto.Name
            };

            _context.VehicleTypes.Add(vehicleType);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTypeById), new { id = vehicleType.Id }, vehicleType);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<VehicleType>> GetTypeById(int id)
        {
            var vehicleType = await _context.VehicleTypes.FindAsync(id);

            if (vehicleType == null)
            {
                return NotFound("Araç tipi bulunamadı.");
            }

            return Ok(vehicleType);
        }
    }
}
