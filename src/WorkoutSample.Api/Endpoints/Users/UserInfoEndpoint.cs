using FastEndpoints;
using Microsoft.AspNetCore.Identity;
using WorkoutSample.Domain;

namespace WorkoutSample.Api.Endpoints.Users;

public class UserInfoEndpoint(UserManager<ApplicationUser> userManager) : EndpointWithoutRequest<
    UserInfoEndpoint.UserInfoResponse>
{
    public override void Configure()
    {
        Get("/users/userinfo");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var userId = Guid.Parse(User.Claims.First(claim => claim.Type == "nameid").Value);
        var user = await userManager.FindByIdAsync(userId.ToString());
        var roles = await userManager.GetRolesAsync(user!);
        await SendOkAsync(new()
        {
            Id = userId,
            Name = user!.Name,
            Email = user.Email!,
            Roles = roles.ToArray()
        }, ct);
    }

    public class UserInfoResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string[] Roles { get; set; } = [];
    }
}