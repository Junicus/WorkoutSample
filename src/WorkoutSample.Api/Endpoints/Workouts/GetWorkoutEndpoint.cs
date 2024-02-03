using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using WorkoutSample.Api.ViewModels;
using WorkoutSample.Infrastructure.Persistence;

namespace WorkoutSample.Api.Endpoints.Workouts;

public class GetWorkoutEndpoint(WorkoutDbContext context) : Endpoint<
    GetWorkoutEndpoint.GetWorkoutRequest,
    GetWorkoutEndpoint.GetWorkoutResponse>
{
    public override void Configure()
    {
        Get("/workouts/{Id}");
    }

    public override async Task HandleAsync(GetWorkoutRequest req, CancellationToken ct)
    {
        var userId = Guid.Parse(User.Claims.First(claim => claim.Type == "nameid").Value);
        var workout = await context.Workouts
            .Include(w => w.Exercises)
            .Where(w => w.UserId == userId && w.Id == req.Id)
            .SingleOrDefaultAsync(ct);
        if (workout == null)
        {
            await SendNotFoundAsync(ct);
        }
        else
        {
            await SendOkAsync(new()
            {
                Workout = new()
                {
                    Id = workout.Id,
                    Date = workout.Date,
                    User = new()
                    {
                        Id = userId,
                        Name = User.Identity?.Name ?? ""
                    },
                    Exercises = workout.Exercises.Select(e => new ExerciseDto()
                    {
                        Id = e.Id,
                        Name = e.Name,
                        Description = e.Description,
                        Reps = e.Reps,
                        Sets = e.Sets,
                        Weight = e.Weight
                    }).ToList()
                }
            }, ct);
        }
    }

    public class GetWorkoutResponse
    {
        public WorkoutDto Workout { get; set; } = new();
    }

    public class GetWorkoutRequest
    {
        public Guid Id { get; set; }
    }
}