namespace CP1.Data.Models;

public class UserRole
{
    public int UserId { get; set; }
    public int RoleId { get; set; }
    public string? Description { get; set; }
    public User? User { get; set; }
    public Role? Role { get; set; }
}