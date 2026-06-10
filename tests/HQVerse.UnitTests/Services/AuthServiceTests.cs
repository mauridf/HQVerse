using FluentAssertions;
using HQVerse.Application.DTOs.Auth;
using HQVerse.Application.Services;
using HQVerse.Domain.Entities;
using HQVerse.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NSubstitute;
using AutoMapper;
using HQVerse.Application.Mappings;
using AutoMapper.Internal;

namespace HQVerse.UnitTests.Services;

public class AuthServiceTests
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;
    private readonly AuthService _sut;

    public AuthServiceTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _logger = Substitute.For<ILogger<AuthService>>();

        // Configurar AutoMapper - API corrigida para versão 16.x
        var mapperConfig = new MapperConfiguration(cfg => {
            cfg.Internal().MethodMappingEnabled = false;
            cfg.AddProfile<MappingProfile>();
        });
        _mapper = mapperConfig.CreateMapper();

        // Configurar Configuration com JWT Secret
        var inMemorySettings = new Dictionary<string, string?>
        {
            { "Jwt:Secret", "HQVerse-Test-JWT-Secret-Key-AtLeast32Chars!!" }
        };
        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        _unitOfWork.Users.Returns(_userRepository);

        _sut = new AuthService(_unitOfWork, _mapper, _configuration, _logger);
    }

    [Fact]
    public async Task RegisterAsync_WithNewUser_ReturnsAuthResponse()
    {
        // Arrange
        var dto = new RegisterDto
        {
            Username = "testuser",
            DisplayName = "Test User",
            Email = "test@email.com",
            Password = "StrongPass123!"
        };

        _userRepository.GetByEmailAsync(dto.Email).Returns((User?)null);
        _userRepository.GetByUsernameAsync(dto.Username).Returns((User?)null);
        _userRepository.AddAsync(Arg.Any<User>()).Returns(callInfo => callInfo.Arg<User>());
        _unitOfWork.SaveChangesAsync().Returns(1);

        // Act
        var result = await _sut.RegisterAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.AccessToken.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBeNullOrEmpty();
        result.User.Username.Should().Be("testuser");
        result.User.Email.Should().Be("test@email.com");
    }

    [Fact]
    public async Task RegisterAsync_WithExistingEmail_ThrowsException()
    {
        // Arrange
        var dto = new RegisterDto
        {
            Username = "testuser",
            DisplayName = "Test User",
            Email = "existing@email.com",
            Password = "StrongPass123!"
        };

        var existingUser = new User { Email = "existing@email.com" };
        _userRepository.GetByEmailAsync(dto.Email).Returns(existingUser);

        // Act
        var act = () => _sut.RegisterAsync(dto);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Email already registered.");
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ReturnsAuthResponse()
    {
        // Arrange
        var dto = new LoginDto
        {
            Email = "test@email.com",
            Password = "StrongPass123!"
        };

        var user = new User
        {
            Id = 1,
            Username = "testuser",
            DisplayName = "Test User",
            Email = "test@email.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("StrongPass123!"),
            Role = "User"
        };

        _userRepository.GetByEmailAsync(dto.Email).Returns(user);
        _unitOfWork.SaveChangesAsync().Returns(1);

        // Act
        var result = await _sut.LoginAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.AccessToken.Should().NotBeNullOrEmpty();
        result.User.Email.Should().Be("test@email.com");
    }

    [Fact]
    public async Task LoginAsync_WithInvalidPassword_ThrowsUnauthorized()
    {
        // Arrange
        var dto = new LoginDto
        {
            Email = "test@email.com",
            Password = "WrongPassword!"
        };

        var user = new User
        {
            Id = 1,
            Email = "test@email.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("StrongPass123!")
        };

        _userRepository.GetByEmailAsync(dto.Email).Returns(user);

        // Act
        var act = () => _sut.LoginAsync(dto);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Invalid email or password.");
    }

    [Fact]
    public async Task RefreshTokenAsync_WithExpiredToken_ThrowsUnauthorized()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Email = "test@email.com",
            RefreshToken = "expired-token",
            RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(-1)
        };

        _userRepository.GetByRefreshTokenAsync("expired-token").Returns(user);

        // Act
        var act = () => _sut.RefreshTokenAsync("expired-token");

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Invalid or expired refresh token.");
    }
}