using Microsoft.AspNetCore.Mvc;
using Otopark.DbContext;
using Otopark.DTO;
using Otopark.Models;

namespace Otopark.Controller;

[ApiController]
[Route("api/[controller]")]

public class UserController : ControllerBase
{
    public ApplicationDbContext _context;

    public UserController(ApplicationDbContext context)
    {
        this._context = context;
    }


    [HttpPost]
    public ActionResult<User> AddUser(UserDto userDto)
    {
        try
        {
            var user = new User
            {
                Name = userDto.Name,
                Username = userDto.Username,
                Surname = userDto.Surname,
                Password = BCrypt.Net.BCrypt.HashPassword(userDto.Password)
            };


            _context.Users.Add(user);
            _context.SaveChanges();


            return Ok(user);
        }
        catch (Exception e)
        {

            Console.WriteLine($"Hata oluştu: {e.Message}");
            return StatusCode(500, "Internal server error"); // Sunucu hatası
        }
    }

    [HttpGet]
    public ActionResult<List<User>> GetUsers()
    {
        try
        {
            var user = _context.Users.ToList();
            return Ok(user);

        }
        catch (Exception e)
        {

            Console.WriteLine($"Hata oluştu: {e.Message}");
            return StatusCode(500, "Internal server error");
        }
    }


    [HttpGet("{id}")]
    public ActionResult<User> GetUser(int id)
    {
        try
        {
            var user = _context.Users.FirstOrDefault(x => x.Id == id);
            return Ok(user);

        }
        catch (Exception e)
        {

            Console.WriteLine($"Hata oluştu: {e.Message}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("{id}")]
    public ActionResult<User> DeleteUser(int id)
    {
        try
        {
            var user = _context.Users.FirstOrDefault(x => x.Id == id);
            _context.Users.Remove(user);
            _context.SaveChanges();
            return Ok("silme işlemi başarılı");
        }
        catch (Exception e)
        {

            Console.WriteLine($"Hata oluştu: {e.Message}");
            return StatusCode(500, "Internal server error");
        }
    }
    
    [HttpPut("{id}")]
    public ActionResult<User> UpdateUser(int id, UserDto userDto)
    {
        try
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);

            if (user == null)
            {
                return NotFound("User not found");
            }
            
            user.Name = userDto.Name;
            user.Username = userDto.Username;
            user.Surname = userDto.Surname;
            
            if (!string.IsNullOrEmpty(userDto.Password))
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(userDto.Password);
            }
            
            _context.SaveChanges();

            return Ok(user); 
        }
        catch (Exception e)
        {
            Console.WriteLine($"Hata oluştu: {e.Message}");
            return StatusCode(500, "Internal server error");
        }
    }


}
