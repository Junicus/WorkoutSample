using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using WorkoutSample.Domain;
using WorkoutSample.Infrastructure.Persistence;

namespace WorkoutSample.Api.Endpoints.Workouts;

public class AddExerciseEndpoint(WorkoutDbContext context) : Endpoint<
    AddExerciseEndpoint.AddExerciseRequest,
    AddExerciseEndpoint.AddExerciseResponse
>
{
    public override void Configure()
    {
        Post("/workouts/{WorkoutId}/exercises");
    }

    public override async Task HandleAsync(AddExerciseRequest req, CancellationToken ct)
    {
        var userId = Guid.Parse(User.Claims.First(claim => claim.Type == "nameid").Value);

        var existingWorkout =
            await context.Workouts.SingleOrDefaultAsync(x => x.UserId == userId && x.Id == req.WorkoutId, ct);
        if (existingWorkout is { } workout)
        {
            var newExercise = new Exercise()
            {
                WorkoutId = workout.Id,
                Name = req.Name,
                Description = req.Description,
                Reps = req.Reps,
                Sets = req.Sets,
                Weight = req.Weight
            };
            await context.Exercises.AddAsync(newExercise, ct);
            await context.SaveChangesAsync(ct);

            await SendCreatedAtAsync<GetExerciseEndpoint>(
                new
                {
                    WorkoutId = workout.Id,
                    ExerciseId = newExercise.Id
                },
                new()
                {
                    WorkoutId = workout.Id,
                    ExerciseId = newExercise.Id
                }, generateAbsoluteUrl: true, cancellation: ct);
        }
        else
        {
            await SendNotFoundAsync(ct);
        }
    }

    public class AddExerciseResponse
    {
        public Guid WorkoutId { get; set; }
        public Guid ExerciseId { get; set; }
    }

    public class AddExerciseRequest
    {
        public Guid WorkoutId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Reps { get; set; }
        public int Sets { get; set; }
        public int Weight { get; set; }
    }
}