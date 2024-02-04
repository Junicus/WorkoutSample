namespace WorkoutSample.Maui.Models;

public class LoginModel
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginResponseModel
{
    public string TokenType { get; set; } = string.Empty;
    public string AccessToken { get; set; } = string.Empty;
    public int ExpiresIn { get; set; }
    public Guid RefreshToken { get; set; }
}

public class RefreshTokenRequestModel
{
    public string AccessToken { get; set; } = string.Empty;
    public Guid RefreshToken { get; set; }
}

public class RefreshTokenResponseModel
{
    public string TokenType { get; set; } = string.Empty;
    public string AccessToken { get; set; } = string.Empty;
    public int ExpiresIn { get; set; }
    public Guid RefreshToken { get; set; }
}

public class RegisterUserModel
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class UserInfoModel
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}