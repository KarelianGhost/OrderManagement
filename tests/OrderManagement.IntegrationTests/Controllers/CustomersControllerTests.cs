using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using OrderManagement.Application.DTOs;
using OrderManagement.Application.DTOs.Auth;
using OrderManagement.IntegrationTests.Fixtures;

namespace OrderManagement.IntegrationTests.Controllers;

[Collection("Integration")]
public class CustomersControllerTests
{
    private readonly IntegrationTestFixture _fixture;

    public CustomersControllerTests(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
    }

    private async Task<HttpClient> CreateAuthenticatedClientAsync()
    {
        var client = _fixture.Factory.CreateClient();

        var loginDto = new LoginDto { Email = "admin@example.com", Password = "Admin123!" };
        var response = await client.PostAsJsonAsync("/api/v1/auth/login", loginDto);
        response.EnsureSuccessStatusCode();

        var auth = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", auth!.Token);

        return client;
    }

    [Fact]
    public async Task Create_ValidCustomer_ReturnsCreated()
    {
        await _fixture.ResetDatabaseAsync();
        using var client = await CreateAuthenticatedClientAsync();

        var dto = new CreateCustomerDto
        {
            FirstName = "Иван",
            LastName = "Петров",
            Email = $"test-{Guid.NewGuid()}@example.com",
            PhoneNumber = "+79161234567"
        };

        var response = await client.PostAsJsonAsync("/api/v1/customers", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var created = await response.Content.ReadFromJsonAsync<CustomerDto>();
        created.Should().NotBeNull();
        created!.Email.Should().Be(dto.Email);
    }

    [Fact]
    public async Task Create_InvalidEmail_ReturnsBadRequest()
    {
        await _fixture.ResetDatabaseAsync();
        using var client = await CreateAuthenticatedClientAsync();

        var dto = new CreateCustomerDto
        {
            FirstName = "Иван",
            LastName = "Петров",
            Email = "not-an-email",
            PhoneNumber = "123"
        };

        var response = await client.PostAsJsonAsync("/api/v1/customers", dto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetAll_WithoutAuth_ReturnsUnauthorized()
    {
        await _fixture.ResetDatabaseAsync();
        using var client = _fixture.Factory.CreateClient();

        var response = await client.GetAsync("/api/v1/customers");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}