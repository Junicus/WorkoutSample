using FastEndpoints;

namespace WorkoutSample.Api.Endpoints.Users;

public sealed class UsersGroup : Group
{
    public UsersGroup()
    {
        Configure("users", ep => { ep.Description(x => x.WithTags("Users")); });
    }
}