using CP1.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataUserRole = CP1.Data.Models.UserRole;

namespace CP1.Api.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly Cp1Context _db;
        public UsersController(Cp1Context db) => _db = db;

        [HttpPut("{userId:int}/role/{roleId:int}")]
        public async Task<IActionResult> AssignRole(int userId, int roleId)
        {
            var userExists = await _db.Users.AnyAsync(u => u.UserId == userId && u.IsActive);
            if (!userExists) return NotFound("User not found.");

            var roleExists = await _db.Roles.AnyAsync(r => r.RoleId == roleId);
            if (!roleExists) return NotFound("Role not found.");

            var current = await _db.UserRoles.Where(x => x.UserId == userId).ToListAsync();
            _db.UserRoles.RemoveRange(current);
            _db.UserRoles.Add(new DataUserRole
            {
                UserId = userId,
                RoleId = roleId,
                Description = "Assigned via API"
            });

            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}