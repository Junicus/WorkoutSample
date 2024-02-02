using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using WorkoutSample.Api.ViewModels;
using WorkoutSample.Infrastructure.Persistence;

namespace WorkoutSample.Api.Endpoints.Workouts;

public class GetWorkoutsEndpoint(WorkoutDbContext context) : Endpoint<
    GetWorkoutsEndpoint.WorkoutsRequest,
    GetWorkoutsEndpoint.WorkoutsResponse>
{
    public override void Configure()
    {
        Get("/");
        Group<WorkoutsGroup>();
    }

    public override async Task HandleAsync(WorkoutsRequest req, CancellationToken ct)
    {
        var userId = Guid.Parse(User.Claims.First(claim => claim.Type == "nameid").Value);
        var workouts = await context.Workouts
            .Include(w => w.Exercises)
            .Where(workout => workout.UserId == userId && workout.Date >= req.FromDate && workout.Date <= req.ToDate)
            .ToListAsync(ct);
        await SendOkAsync(new()
        {
            Workouts = workouts.Select(w => new WorkoutDto
            {
                Id = w.Id,
                Date = w.Date,
                Exercises = w.Exercises.Select(e => new ExerciseDto
                {
                    Id = e.Id,
                    Name = e.Name,
                    Description = e.Description,
                    Reps = e.Reps,
                    Sets = e.Sets,
                    Weight = e.Weight
                }).ToList()
            })
        }, ct);
    }

    public class WorkoutsResponse
    {
        public IEnumerable<WorkoutDto> Workouts { get; set; } = [];
    }

    public class WorkoutsRequest
    {
        [QueryParam] public DateOnly FromDate { get; set; }
        [QueryParam] public DateOnly ToDate { get; set; }
    }
}