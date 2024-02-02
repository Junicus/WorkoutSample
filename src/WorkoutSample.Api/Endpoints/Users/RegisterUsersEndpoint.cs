using FastEndpoints;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using WorkoutSample.Domain;

namespace WorkoutSample.Api.Endpoints.Users;

public class RegisterUsersEndpoint(UserManager<ApplicationUser> userManager) : Endpoint<
    RegisterUsersEndpoint.RegisterUserRequest,
    RegisterUsersEndpoint.RegisterUserResponse>
{
    public override void Configure()
    {
        Post("/register");
        AllowAnonymous();
        Group<UsersGroup>();
    }

    public override async Task HandleAsync(RegisterUserRequest req, CancellationToken ct)
    {
        var user = await userManager.FindByEmailAsync(req.Email);
        if (user != null)
        {
            throw new ValidationException(new[]
            {
                new ValidationFailure(nameof(ApplicationUser), "Email address already in use")
            });
        }

        var newUser = new ApplicationUser()
        {
            Name = req.Name,
            Email = req.Email,
            UserName = req.Email
        };

        try
        {
            var createdUserResult = await userManager.CreateAsync(newUser, req.Password);
            if (!createdUserResult.Succeeded)
            {
                throw new ValidationException(createdUserResult.Errors.Select(error =>
                    new ValidationFailure("AuthError", error.Description)));
            }

            await SendAsync(new());
        }
        catch (Exception ex)
        {
            throw new ValidationException(new[]
            {
                new ValidationFailure(nameof(ApplicationUser), "Could not register user")
            });
        }
    }

    public class RegisterUserResponse
    {
    }

    public class RegisterUserRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}