using MediatR;
using Microsoft.EntityFrameworkCore;
using WorkoutSample.Application.Workouts.Queries.GetWorkouts;
using WorkoutSample.Infrastructure.Persistence;

namespace WorkoutSample.Application.Exercises.Queries.GetExcercise;

public record GetExercise : IRequest<ExerciseDto>
{
    public Guid Id { get; set; }
}

public class GetExerciseHandler(WorkoutDbContext context) : IRequestHandler<GetExercise, ExerciseDto>
{
    public async Task<ExerciseDto> Handle(GetExercise request, CancellationToken cancellationToken)
    {
        var entity = await context.Exercises.SingleAsync(e => e.Id == request.Id, cancellationToken);

        return new ExerciseDto
        {
            Name = entity.Name,
            Description = entity.Description,
            Reps = entity.Reps,
            Sets = entity.Sets,
            Weight = entity.Weight
        };
    }
}