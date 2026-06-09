using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace HQVerse.IntegrationTests.Api;

public class HealthEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public HealthEndpointTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task HealthEndpoint_ReturnsOk()
    {
        // Act
        var response = await _client.GetAsync("/api/health");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task HealthEndpoint_ReturnsJsonWithStatus()
    {
        // Act
        var response = await _client.GetAsync("/api/health");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        content.Should().Contain("Healthy");
        content.Should().Contain("Version");
    }
}