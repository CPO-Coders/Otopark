using Microsoft.AspNetCore.Mvc;
using Otopark.DbContext;
using Otopark.DTOs;
using Otopark.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace Otopark.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CarController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        
        private readonly ILogger<CarController> _logger;

        public CarController(ApplicationDbContext context, ILogger<CarController> logger)
        {
            _context = context;
            _logger = logger;
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
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<List<Car>>> GetAllCars()
        {
            try
            {
                var car = _context.Cars.ToList();
                return Ok(car);
            }
            catch (Exception e)
            {
                
                return BadRequest(e.Message);
            }
        }
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCar(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car == null)
            {
                return NotFound("Araba bulunamadı.");
            }
    
            var tickets = await _context.Tickets.Where(t => t.CarId == id).ToListAsync();
            
            foreach (var ticket in tickets)
            {
                var deleteTime = DateTime.UtcNow;
        
                // Çıkış zamanını ve ücreti güncelle
                ticket.ExitTime = DateTime.UtcNow;
                var timeSpent = ticket.ExitTime.Value - ticket.EntryTime;
                var fee = (decimal)(timeSpent.TotalHours * 10);
                ticket.Fee = fee;
                
                ticket.CarId = null;

                ticket.Status = "false"; //Ticket false alındı pasif artık
                
                _context.Tickets.Update(ticket);
        
                // Loglama işlemi
                _logger.LogInformation(
                    $"Silinen Ticket: Id = {ticket.Id}, CarId = {ticket.CarId}, SpotId = {ticket.SpotId}, EntryTime = {ticket.EntryTime}, ExitTime = {ticket.ExitTime}, Fee = {ticket.Fee}, Status = {ticket.Status}, DeletedTime = {deleteTime}");
            }
            
            _context.Cars.Remove(car);
            await _context.SaveChangesAsync();

            return NoContent();
        }



    }
}
