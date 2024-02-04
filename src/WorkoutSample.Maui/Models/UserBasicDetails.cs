namespace WorkoutSample.Maui.Models;

public class UserBasicDetails
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string AccessToken { get; set; } = string.Empty;
    public Guid RefreshToken { get; set; }
}