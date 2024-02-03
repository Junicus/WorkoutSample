using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FastEndpoints;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WorkoutSample.Api.Options;
using WorkoutSample.Domain;
using WorkoutSample.Infrastructure.Persistence;
using ValidationFailure = FluentValidation.Results.ValidationFailure;

namespace WorkoutSample.Api.Endpoints.Users;

public class RefreshTokenEndpoint(
    UserManager<ApplicationUser> userManager,
    WorkoutDbContext context,
    IOptions<JwtOptions> options) : Endpoint<
    RefreshTokenEndpoint.RefreshTokenRequest,
    RefreshTokenEndpoint.RefreshTokenResponse
>
{
    public override void Configure()
    {
        Post("/users/refresh");
        AllowAnonymous();
    }

    public override async Task HandleAsync(RefreshTokenRequest req, CancellationToken ct)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var claims = tokenHandler.ReadJwtToken(req.AccessToken).Claims.ToList();
        if (claims.Count <= 0)
        {
            AddError(new ValidationFailure(nameof(RefreshTokenRequest.AccessToken), "Invalid access token."));
            await SendErrorsAsync(cancellation: ct);
            return;
        }

        var userId = claims.Single(c => c.Type == "nameid").Value;
        var existingUser = await userManager.FindByIdAsync(userId);

        if (existingUser == null)
        {
            AddError(new ValidationFailure(nameof(RefreshTokenRequest.AccessToken), "Invalid access token."));
            await SendNotFoundAsync(cancellation: ct);
            return;
        }

        var existingRefreshToken = await context.RefreshTokens
            .Where(r => r.UserId == existingUser!.Id && r.Id == req.RefreshToken)
            .SingleOrDefaultAsync(ct);

        if (existingRefreshToken == null)
        {
            AddError(new ValidationFailure(nameof(RefreshTokenRequest.RefreshToken), "Invalid refresh token."));
            await SendErrorsAsync(StatusCodes.Status404NotFound, cancellation: ct);
            return;
        }

        if (existingRefreshToken.IsUsed || existingRefreshToken.IsInvalidated ||
            existingRefreshToken.ExpiresAt <= DateTime.Now)
        {
            AddError(new ValidationFailure(nameof(RefreshTokenRequest.RefreshToken), "Invalid refresh token."));
            await SendErrorsAsync(cancellation: ct);
            return;
        }

        existingRefreshToken.IsUsed = true;

        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = existingUser.Id,
            ExpiresAt = DateTime.Now.AddDays(7)
        };
        await context.RefreshTokens.AddAsync(refreshToken, ct);
        await context.SaveChangesAsync(ct);

        var userRoleNames = await userManager.GetRolesAsync(existingUser);
        var allClaims = new List<Claim>();
        allClaims.AddRange(userRoleNames.Select(role => new Claim("role", role)));
        allClaims.AddRange(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, existingUser.Id.ToString()),
            new Claim("email", existingUser.Email!),
            new Claim("name", existingUser.Name),
        });

        var secret = Encoding.ASCII.GetBytes(options.Value.Key);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(allClaims),
            Expires = DateTime.UtcNow.AddMinutes(15),
            SigningCredentials = new(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256Signature)
        };

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

    public class RefreshTokenResponse
    {
        public string TokenType { get; set; } = string.Empty;
        public string AccessToken { get; set; } = string.Empty;
        public int ExpiresIn { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
    }

    public class RefreshTokenRequest
    {
        public string AccessToken { get; set; } = string.Empty;
        public Guid RefreshToken { get; set; }
    }
}