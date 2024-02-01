using MediatR;
using Microsoft.EntityFrameworkCore;
using WorkoutSample.Application.Workouts.Queries.GetWorkouts;
using WorkoutSample.Infrastructure.Persistence;

namespace WorkoutSample.Application.Workouts.Queries.GetWorkout;

public record GetWorkout : IRequest<WorkoutDto>
{
    public Guid Id { get; init; }
}

public class GetWorkoutHandler(WorkoutDbContext context) : IRequestHandler<GetWorkout, WorkoutDto>
{
    public async Task<WorkoutDto> Handle(GetWorkout request, CancellationToken cancellationToken)
    {
        var workout = await context.Workouts
            .Where(w => w.Id == request.Id)
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
            .SingleAsync(cancellationToken);

        return workout;
    }
}