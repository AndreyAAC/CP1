namespace CP1.Models.DTOs
{
    public class UserRoleDTO
    {
        public int UserId { get; set; }
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? FullName { get; set; }

        public int RoleId { get; set; }     // rol actual
        public string RoleName { get; set; } = "";
    }
}