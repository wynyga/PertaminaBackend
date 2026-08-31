using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Data.Repositories;
using Models.DTOs;

namespace Business.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<LoginResponseDTO?> LoginAsync(LoginRequestDTO request)
        {
            var user = await _userRepository.GetUserByEmailAsync(request.Email);
            
            // Password verification using BCrypt
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return null;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var secret = Environment.GetEnvironmentVariable("JwtSettings__Secret") ?? "";
            var issuer = Environment.GetEnvironmentVariable("JwtSettings__Issuer");
            var audience = Environment.GetEnvironmentVariable("JwtSettings__Audience");
            var expirationStr = Environment.GetEnvironmentVariable("JwtSettings__ExpirationInMinutes") ?? "60";
            
            var key = Encoding.UTF8.GetBytes(secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email)
                }),
                Expires = DateTime.UtcNow.AddMinutes(double.Parse(expirationStr)),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            
            return new LoginResponseDTO
            {
                Token = tokenHandler.WriteToken(token),
                ExpiresIn = int.Parse(expirationStr) * 60
            };
        }

        public async Task<UserProfileDTO?> GetProfileAsync(int userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null) return null;

            return new UserProfileDTO
            {
                Id = user.Id,
                Email = user.Email
            };
        }

        public async Task<UserProfileDTO?> RegisterAsync(RegisterRequestDTO request)
        {
            // Cek apakah email sudah terdaftar
            var existingUser = await _userRepository.GetUserByEmailAsync(request.Email);
            if (existingUser != null)
            {
                return null; 
            }

            var user = new Models.Entities.User
            {
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
            };

            await _userRepository.AddUserAsync(user);

            return new UserProfileDTO
            {
                Id = user.Id,
                Email = user.Email
            };
        }
    }
}

