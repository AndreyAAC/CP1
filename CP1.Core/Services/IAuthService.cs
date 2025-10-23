using CP1.Models.DTOs;

namespace CP1.Core.Services
{
    public interface IAuthService
    {
        Task<UserDTO?> LoginAsync(string email, string password, CancellationToken ct = default);
    }
}