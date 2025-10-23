using CP1.Data.Models;
using CP1.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CP1.Api.Controllers
{
    [ApiController]
    [Route("api/roles")]
    public class RolesController : ControllerBase
    {
        private readonly Cp1Context _db;
        public RolesController(Cp1Context db) => _db = db;

        [HttpPost]
        public async Task<ActionResult<RoleDTO>> Create([FromBody] RoleDTO dto)
        {
            if (dto is null || string.IsNullOrWhiteSpace(dto.RoleName))
                return BadRequest("RoleName is required.");

            var exists = await _db.Roles.AnyAsync(r => r.RoleName == dto.RoleName);
            if (exists) return Conflict("RoleName already exists.");

            var role = new Role { RoleName = dto.RoleName, Description = dto.Description };
            _db.Roles.Add(role);
            await _db.SaveChangesAsync();

            dto.RoleId = role.RoleId;
            return CreatedAtAction(nameof(GetById), new { id = role.RoleId }, dto);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<RoleDTO>> GetById(int id)
        {
            var r = await _db.Roles.AsNoTracking().FirstOrDefaultAsync(x => x.RoleId == id);
            if (r is null) return NotFound();
            return new RoleDTO { RoleId = r.RoleId, RoleName = r.RoleName, Description = r.Description };
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] RoleDTO dto)
        {
            var role = await _db.Roles.FirstOrDefaultAsync(r => r.RoleId == id);
            if (role is null) return NotFound();

            if (!string.IsNullOrWhiteSpace(dto.RoleName) && !string.Equals(role.RoleName, dto.RoleName, StringComparison.Ordinal))
            {
                var exists = await _db.Roles.AnyAsync(r => r.RoleName == dto.RoleName && r.RoleId != id);
                if (exists) return Conflict("RoleName already exists.");
                role.RoleName = dto.RoleName;
            }
            role.Description = dto.Description;
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}