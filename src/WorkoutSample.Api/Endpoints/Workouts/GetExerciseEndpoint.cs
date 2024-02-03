using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using WorkoutSample.Api.ViewModels;
using WorkoutSample.Infrastructure.Persistence;

namespace WorkoutSample.Api.Endpoints.Workouts;

public class GetExerciseEndpoint(WorkoutDbContext context) : Endpoint<
    GetExerciseEndpoint.GetExerciseRequest,
    GetExerciseEndpoint.GetExerciseResponse
>
{
    public override void Configure()
    {
        Get("/workouts/{WorkoutId}/exercises/{ExerciseId}");
    }

    public override async Task HandleAsync(GetExerciseRequest req, CancellationToken ct)
    {
        var userId = Guid.Parse(User.Claims.First(claim => claim.Type == "nameid").Value);
        var existingWorkout = await context.Workouts
            .Include(w => w.Exercises)
            .Where(w => w.UserId == userId && w.Id == req.WorkoutId)
            .SingleOrDefaultAsync(ct);
        if (existingWorkout is { } workout)
        {
            var existingExercise = workout.Exercises.SingleOrDefault(e => e.Id == req.ExerciseId);
            if (existingExercise is { } exercise)
            {
                await SendOkAsync(new()
                {
                    Exercise = new()
                    {
                        Id = exercise.Id,
                        Name = exercise.Name,
                        Description = exercise.Description,
                        Reps = exercise.Reps,
                        Sets = exercise.Sets,
                        Weight = exercise.Sets
                    }
                }, ct);
            }
            else
            {
                await SendNotFoundAsync(ct);
            }
        }
        else
        {
            await SendNotFoundAsync(ct);
        }
    }

    public class GetExerciseResponse
    {
        public ExerciseVm Exercise { get; set; } = new();
    }

    public class GetExerciseRequest
    {
        public Guid WorkoutId { get; set; }
        public Guid ExerciseId { get; set; }
    }
}