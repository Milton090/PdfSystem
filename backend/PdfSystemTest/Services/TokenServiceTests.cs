using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using PdfSystem.Services;
using System.IdentityModel.Tokens.Jwt;

public class TokenServiceTests
{
    private IConfiguration GetConfiguration(String jwtSecret)
    {
        var settings = new Dictionary<String, String>
        {
            {"AppSettings:JWT_SECRET", jwtSecret}
        };
        return new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .Build();
    }

    [Fact]
    public void Constructor_ThrowsArgumentException_WhenJWTSecretIsTooShort()
    {
        var invalidConfig = GetConfiguration("short_key");

        Assert.Throws<ArgumentException>(() => new TokenService(invalidConfig));
    }

    [Fact]
    public void GenerateToken_ReturnsValidToken()
    {
        var user = new IdentityUser { Email = "test@test.com" };
        var validConfig = GetConfiguration("my_super_ultra_secret_key_1234567890");

        var tokenService = new TokenService(validConfig);

        var token = tokenService.GenerateToken(user);
        var result = tokenService.ValidateToken(token);

        Assert.NotNull(token);
        Assert.True(result);
    }


    [Fact]
    public void ValidateToken_ReturnsFalse_WhenTokenIsInvalid()
    {
        var user = new IdentityUser { Email = "hi@gmail.com" };
        var config = GetConfiguration("my_super_ultra_secret_key_1234567890");

        var tokenService = new TokenService(config);
        var token = tokenService.GenerateToken(user);
        var invalidToken = token + "invalid";

        var result = tokenService.ValidateToken(invalidToken);

        Assert.False(result);
    }
}