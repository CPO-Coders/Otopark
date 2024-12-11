using Microsoft.AspNetCore.Mvc;
using Otopark.DbContext;
using Otopark.DTOs;
using Otopark.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Otopark.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CarController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CarController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<CarDto>> AddCar(CarDto carDto)
        {
            // Kullanıcı bilgilerini al
            var usedIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (usedIdClaim == null)
            {
                return Unauthorized("Kullanıcı bilgisi alınamadı.");
            }

            int id = int.Parse(usedIdClaim.Value);

            // Kullanıcıyı doğrula
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return BadRequest("Geçersiz kullanıcı ID.");
            }

            // TypeId'nin doğruluğunu kontrol et
            var vehicleType = await _context.VehicleTypes.FirstOrDefaultAsync(vt => vt.Id == carDto.TypeId);
            if (vehicleType == null)
            {
                return BadRequest($"Geçersiz TypeId: {carDto.TypeId}");
            }

            // Yeni araba oluştur
            var car = new Car
            {
                Plate = carDto.Plate,
                mark = carDto.mark,
                TypeId = carDto.TypeId,
                UserId = id
            };

            _context.Cars.Add(car);
            await _context.SaveChangesAsync();

            // Yanıt olarak döndürülecek DTO oluştur
            var carDtoResponse = new CarDto
            {
                Plate = car.Plate,
                mark = car.mark,
                TypeId = car.TypeId
            };

            return CreatedAtAction(nameof(GetCarById), new { id = car.Id }, carDtoResponse);
        }


        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<CarDto>> UpdateCar(int id, CarDto carDto)
        {
            // Kullanıcı bilgilerini al
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized("Kullanıcı bilgisi alınamadı.");
            }

            int userId = int.Parse(userIdClaim.Value);

            // Kullanıcıyı doğrula
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return BadRequest("Geçersiz kullanıcı ID.");
            }

            // Güncellenecek arabayı bul
            var car = await _context.Cars.Include(c => c.VehicleType).FirstOrDefaultAsync(c => c.Id == id);
            if (car == null)
            {
                return NotFound("Araç bulunamadı.");
            }

            // Kullanıcı doğrulaması (Arabayı güncellemek isteyen kullanıcı, arabanın sahibiyse izin verilir)
            if (car.UserId != userId)
            {
                return Forbid("Bu aracı güncelleme yetkiniz yok.");
            }

            // TypeId'nin doğruluğunu kontrol et
            var vehicleType = await _context.VehicleTypes.FirstOrDefaultAsync(vt => vt.Id == carDto.TypeId);
            if (vehicleType == null)
            {
                return BadRequest($"Geçersiz TypeId: {carDto.TypeId}");
            }

            // Araba bilgilerini güncelle
            car.Plate = carDto.Plate;
            car.mark = carDto.mark;
            car.TypeId = carDto.TypeId;

            _context.Cars.Update(car);
            await _context.SaveChangesAsync();

            // Yanıt olarak döndürülecek DTO oluştur
            var carDtoResponse = new CarDto
            {
                Plate = car.Plate,
                mark = car.mark,
                TypeId = car.TypeId
            };

            return Ok(carDtoResponse);
        }

        // Araç Bilgilerini Getir
        [HttpGet("{id}")]
        public async Task<ActionResult<CarDto>> GetCarById(int id)
        {
            var car = await _context.Cars.Include(c => c.VehicleType).FirstOrDefaultAsync(c => c.Id == id);

            if (car == null)
            {
                return NotFound("Araç bulunamadı.");
            }

            var carDtoResponse = new CarDto
            {
                Plate = car.Plate,
                mark = car.mark,
                TypeId = car.TypeId,
            };

            return Ok(carDtoResponse);
        }

        // Araç Silme
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCar(int id)
        {
            var car = await _context.Cars.Include(c => c.VehicleType).FirstOrDefaultAsync(c => c.Id == id);

            if (car == null)
            {
                return NotFound("Araç bulunamadı.");
            }

            _context.Cars.Remove(car);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
