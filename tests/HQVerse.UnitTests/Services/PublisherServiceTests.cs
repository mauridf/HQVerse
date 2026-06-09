using FluentAssertions;
using HQVerse.Application.DTOs;
using HQVerse.Application.DTOs.Publishers;
using HQVerse.Application.Services;
using HQVerse.Domain.Entities;
using HQVerse.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using NSubstitute;
using AutoMapper;
using HQVerse.Application.Mappings;

namespace HQVerse.UnitTests.Services;

public class PublisherServiceTests
{
    private readonly IPublisherRepository _publisherRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<PublisherService> _logger;
    private readonly PublisherService _sut;

    public PublisherServiceTests()
    {
        _publisherRepository = Substitute.For<IPublisherRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _logger = Substitute.For<ILogger<PublisherService>>();

        // Configurar AutoMapper real
        _mapper = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>())
            .CreateMapper();

        // Configurar UnitOfWork para retornar o repositório mockado
        _unitOfWork.Publishers.Returns(_publisherRepository);

        _sut = new PublisherService(_unitOfWork, _mapper, _logger);
    }

    [Fact]
    public async Task GetByIdAsync_WhenPublisherExists_ReturnsPublisherDto()
    {
        // Arrange
        var publisher = new Publisher
        {
            Id = 1,
            Name = "DC Comics",
            Description = "DC Comics description",
            Country = "USA",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _publisherRepository.GetByIdAsync(1).Returns(publisher);

        // Act
        var result = await _sut.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("DC Comics");
        result.Country.Should().Be("USA");
        result.Id.Should().Be(1);
    }

    [Fact]
    public async Task GetByIdAsync_WhenPublisherDoesNotExist_ReturnsNull()
    {
        // Arrange
        _publisherRepository.GetByIdAsync(999).Returns((Publisher?)null);

        // Act
        var result = await _sut.GetByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_WithValidData_CreatesPublisher()
    {
        // Arrange
        var dto = new CreatePublisherDto
        {
            Name = "Marvel Comics",
            Description = "House of Ideas",
            Country = "USA"
        };

        _publisherRepository.AddAsync(Arg.Any<Publisher>()).Returns(callInfo => callInfo.Arg<Publisher>());
        _unitOfWork.SaveChangesAsync().Returns(1);

        // Act
        var result = await _sut.CreateAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Marvel Comics");
        result.Description.Should().Be("House of Ideas");

        await _publisherRepository.Received(1).AddAsync(Arg.Is<Publisher>(p => p.Name == "Marvel Comics"));
        await _unitOfWork.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task DeleteAsync_WhenPublisherExists_ReturnsTrue()
    {
        // Arrange
        var publisher = new Publisher { Id = 1, Name = "DC Comics" };
        _publisherRepository.GetByIdAsync(1).Returns(publisher);
        _unitOfWork.SaveChangesAsync().Returns(1);

        // Act
        var result = await _sut.DeleteAsync(1);

        // Assert
        result.Should().BeTrue();
        await _publisherRepository.Received(1).DeleteAsync(publisher);
        await _unitOfWork.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task DeleteAsync_WhenPublisherDoesNotExist_ReturnsFalse()
    {
        // Arrange
        _publisherRepository.GetByIdAsync(999).Returns((Publisher?)null);

        // Act
        var result = await _sut.DeleteAsync(999);

        // Assert
        result.Should().BeFalse();
        await _publisherRepository.DidNotReceive().DeleteAsync(Arg.Any<Publisher>());
    }

    [Fact]
    public async Task GetAllAsync_ReturnsPaginatedResult()
    {
        // Arrange
        var publishers = new List<Publisher>
        {
            new() { Id = 1, Name = "DC Comics" },
            new() { Id = 2, Name = "Marvel" },
            new() { Id = 3, Name = "Image" }
        };

        _publisherRepository.GetAllAsync().Returns(publishers);
        _publisherRepository.CountAsync(Arg.Any<System.Linq.Expressions.Expression<Func<Publisher, bool>>>()).Returns(3);

        var paginationParams = new PaginationParams { Page = 1, PageSize = 2 };

        // Act
        var result = await _sut.GetAllAsync(paginationParams);

        // Assert
        result.Should().NotBeNull();
        result.TotalCount.Should().Be(3);
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(2);
        result.Items.Should().HaveCount(2);
    }
}