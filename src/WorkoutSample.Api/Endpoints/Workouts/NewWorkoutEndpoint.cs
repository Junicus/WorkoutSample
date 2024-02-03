using FastEndpoints;
using WorkoutSample.Domain;
using WorkoutSample.Infrastructure.Persistence;

namespace WorkoutSample.Api.Endpoints.Workouts;

public class NewWorkoutEndpoint(WorkoutDbContext context) : Endpoint<
    NewWorkoutEndpoint.NewWorkoutRequest,
    NewWorkoutEndpoint.NewWorkoutResponse>
{
    public override void Configure()
    {
        Post("/workouts");
    }

    public override async Task HandleAsync(NewWorkoutRequest req, CancellationToken ct)
    {
        var userId = Guid.Parse(User.Claims.First(claim => claim.Type == "nameid").Value);
        var newWorkout = new Workout
        {
            UserId = userId,
            Date = req.WorkoutDate,
            Exercises = []
        };
        await context.Workouts.AddAsync(newWorkout, ct);
        await context.SaveChangesAsync(ct);

        await SendCreatedAtAsync<GetWorkoutsEndpoint>(
            new { Id = newWorkout.Id },
            new()
            {
                Id = newWorkout.Id
            }, generateAbsoluteUrl: true, cancellation: ct);
    }

    public class NewWorkoutResponse
    {
        public Guid Id { get; set; }
    }

    public class NewWorkoutRequest
    {
        public DateOnly WorkoutDate { get; set; }
    }
}