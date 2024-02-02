using FastEndpoints;
using Microsoft.AspNetCore.Identity;
using WorkoutSample.Application.Exceptions;
using WorkoutSample.Domain;
using WorkoutSample.Infrastructure.Persistence;

namespace WorkoutSample.Api.Endpoints.Users;

public class LoginUsersEndpoint(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    WorkoutDbContext context)
    : Endpoint<
        LoginUsersEndpoint.LoginRequest,
        LoginUsersEndpoint.LoginResponse>
{
    public override void Configure()
    {
        Post("/login");
        Group<UsersGroup>();
    }

    public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
    {
        var existingUser = await userManager.FindByEmailAsync(req.Email);
        if (existingUser == null)
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