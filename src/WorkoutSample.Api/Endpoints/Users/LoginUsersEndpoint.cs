using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using FastEndpoints;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WorkoutSample.Api.Options;
using WorkoutSample.Application.Exceptions;
using WorkoutSample.Domain;
using WorkoutSample.Infrastructure.Persistence;

namespace WorkoutSample.Api.Endpoints.Users;

public class LoginUsersEndpoint(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    WorkoutDbContext context,
    IOptions<JwtOptions> options)
    : Endpoint<
        LoginUsersEndpoint.LoginRequest,
        LoginUsersEndpoint.LoginResponse>
{
    public override void Configure()
    {
        Post("/login");
        AllowAnonymous();
        Group<UsersGroup>();
    }

    public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
    {
        var existingUser = await userManager.FindByEmailAsync(req.Email);

        if (existingUser is null)
        {
            throw new NotFoundException(nameof(ApplicationUser), req.Email);
        }

        var result = await signInManager.CheckPasswordSignInAsync(existingUser, req.Password, false);
        if (!result.Succeeded)
        {
            throw new ForbiddenAccessException();
        }

        var userRoleNames = await userManager.GetRolesAsync(existingUser);

        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = existingUser.Id,
            ExpiresAt = DateTime.Now.AddDays(7)
        };

        await context.RefreshTokens.AddAsync(refreshToken, ct);
        await context.SaveChangesAsync(ct);

        var allClaims = new List<Claim>();
        allClaims.AddRange(userRoleNames.Select(role => new Claim("role", role)));
        allClaims.AddRange(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, existingUser.Id.ToString()),
            new Claim("email", existingUser.Email!),
            new Claim("name", existingUser.Name),
        });

        var t = options?.Value?.Key;
        var secret = Encoding.ASCII.GetBytes(options.Value.Key);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(allClaims),
            Expires = DateTime.UtcNow.AddMinutes(15),
            SigningCredentials = new(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        var accessToken = tokenHandler.WriteToken(token);

        await SendOkAsync(new()
        {
            TokenType = "Bearer",
            AccessToken = accessToken,
            ExpiresIn = 15 * 60,
            RefreshToken = refreshToken.Id.ToString()
        }, ct);
    }

    public class LoginResponse
    {
        public string TokenType { get; set; } = string.Empty;
        public string AccessToken { get; set; } = string.Empty;
        public int ExpiresIn { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
    }

    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}