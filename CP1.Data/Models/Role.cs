namespace CP1.Data.Models;

public class Role
{
    public int RoleId { get; set; }
    public string RoleName { get; set; } = null!;
    public string? Description { get; set; }
    public ICollection<UserRole> UserRoles { get; set; } = [];
}