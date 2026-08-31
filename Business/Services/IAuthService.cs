using Models.DTOs;

namespace Business.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDTO?> LoginAsync(LoginRequestDTO request);
        Task<UserProfileDTO?> GetProfileAsync(int userId);
        Task<UserProfileDTO?> RegisterAsync(RegisterRequestDTO request);
    }
}

