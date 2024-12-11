namespace Otopark.DTO;

public class UserDto
{
    
    public string Name { get; set; } = string.Empty;
    
    public string Surname { get; set; } = string.Empty;
    
    public string Username { get; set; } = string.Empty;
    
    public string Password { get; set; } = string.Empty; // Şifreyi hashleyerek

}