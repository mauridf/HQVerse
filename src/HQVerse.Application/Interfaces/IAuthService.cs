using HQVerse.Application.DTOs.Auth;

namespace HQVerse.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto dto, CancellationToken cancellationToken = default);
    Task<AuthResponseDto> LoginAsync(LoginDto dto, CancellationToken cancellationToken = default);
    Task<AuthResponseDto> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task LogoutAsync(int userId, CancellationToken cancellationToken = default);
    Task<UserDto> GetProfileAsync(int userId, CancellationToken cancellationToken = default);
    Task<UserDto> UpdateProfileAsync(int userId, UpdateUserDto dto, CancellationToken cancellationToken = default);
    Task ChangePasswordAsync(int userId, string currentPassword, string newPassword, CancellationToken cancellationToken = default);
}
