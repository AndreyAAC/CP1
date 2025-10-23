namespace CP1.Data.Models;

public class User
{
    public int UserId { get; set; }
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? FullName { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLogin { get; set; }
    public string Password { get; set; } = ""; // ya agregaste la columna
    public ICollection<UserRole> UserRoles { get; set; } = [];
}