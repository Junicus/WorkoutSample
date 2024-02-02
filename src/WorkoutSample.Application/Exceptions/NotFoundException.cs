namespace WorkoutSample.Application.Exceptions;

public class NotFoundException(string name, object key) : Exception($"Entity \"{name}\" {{key}} was not found.")
{
    public string Name { get; private set; } = name;
    public object Key { get; private set; } = key;
}