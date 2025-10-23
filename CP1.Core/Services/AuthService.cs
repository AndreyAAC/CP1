using CP1.Data.Models;
using CP1.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace CP1.Core.Services
{
    public class AuthService : IAuthService
    {
        private readonly Cp1Context _db;
        public AuthService(Cp1Context db) => _db = db;

        public async Task<UserDTO?> LoginAsync(string email, string password, CancellationToken ct = default)
        {
            var user = await _db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email && u.Password == password && u.IsActive, ct);

            if (user is null) return null;

            var role = await _db.UserRoles.AsNoTracking()
                        .Where(ur => ur.UserId == user.UserId)
                        .Join(_db.Roles, ur => ur.RoleId, r => r.RoleId, (ur, r) => r)
                        .FirstOrDefaultAsync(ct);

            return new UserDTO
            {
                UserId = user.UserId,
                Email = user.Email,
                FullName = user.FullName,
                RoleId = role?.RoleId ?? 0,
                RoleName = role?.RoleName ?? ""
            };
        }
    }
}