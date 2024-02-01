using MediatR;
using Microsoft.EntityFrameworkCore;
using WorkoutSample.Infrastructure.Persistence;

namespace WorkoutSample.Application.Workouts.Queries.GetWorkouts;

public record GetWorkouts : IRequest<WorkoutsVm>
{
    public DateOnly Date { get; init; }
}

public class GetWorkoutsHandler(WorkoutDbContext context) : IRequestHandler<GetWorkouts, WorkoutsVm>
{
    public async Task<WorkoutsVm> Handle(GetWorkouts request, CancellationToken cancellationToken)
    {
        var workouts = await context.Workouts
            .Where(w => w.Date == request.Date)
            .Include(w => w.Exercises)
            .Select(w => new WorkoutDto()
            {
                Exercises = w.Exercises.Select(e => new ExerciseDto
                {
                    Name = e.Name,
                    Reps = e.Reps,
                    Sets = e.Sets,
                    Weight = e.Weight
                }).ToList()
            })
            .ToListAsync(cancellationToken);

        return new WorkoutsVm
        {
            Workouts = workouts
        };
    }
}