namespace WorkoutSample.Maui.Models;

public class Settings
{
    public static UserBasicDetails UserBasicDetails { get; set; } = new();
    public const string BaseUrl = "http://localhost:5011";
}