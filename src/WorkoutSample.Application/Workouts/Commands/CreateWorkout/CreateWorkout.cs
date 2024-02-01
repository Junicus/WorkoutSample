using MediatR;
using WorkoutSample.Application.Workouts.Queries.GetWorkouts;
using WorkoutSample.Domain;
using WorkoutSample.Infrastructure.Persistence;

namespace WorkoutSample.Application.Workouts.Commands.CreateWorkout;

public record CreateWorkout : IRequest<Guid>
{
    public Guid UserId { get; set; }
    public DateOnly Date { get; set; }
    public List<ExerciseDto> Exercises { get; init; } = [];
}

public class CreateWorkoutHandler(WorkoutDbContext context) : IRequestHandler<CreateWorkout, Guid>
{
    public async Task<Guid> Handle(CreateWorkout request, CancellationToken cancellationToken)
    {
        var entity = new Workout
        {
            UserId = request.UserId,
            Date = request.Date,
            Exercises = request.Exercises.Select(e => new Exercise
                { Name = e.Name, Sets = e.Sets, Reps = e.Reps, Weight = e.Weight }).ToList(),
        };

        context.Workouts.Add(entity);
        await context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}