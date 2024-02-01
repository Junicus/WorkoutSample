using MediatR;
using WorkoutSample.Domain;
using WorkoutSample.Infrastructure.Persistence;

namespace WorkoutSample.Application.Exercises.Commands.CreateExercise;

public record CreateExercise : IRequest<Guid>
{
    public Guid WorkoutId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Reps { get; set; }
    public int Sets { get; set; }
    public int Weight { get; set; }
}

public class CreateExerciseHandler(WorkoutDbContext context) : IRequestHandler<CreateExercise, Guid>
{
    public async Task<Guid> Handle(CreateExercise request, CancellationToken cancellationToken)
    {
        var entity = new Exercise
        {
            WorkoutId = request.WorkoutId,
            Name = request.Name,
            Description = request.Description,
            Reps = request.Reps,
            Sets = request.Sets,
            Weight = request.Weight
        };

        context.Exercises.Add(entity);

        await context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}