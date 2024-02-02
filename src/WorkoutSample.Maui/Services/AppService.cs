using System.Text;
using System.Text.Json;
using WorkoutSample.Maui.Constants;
using WorkoutSample.Maui.Contracts;
using WorkoutSample.Maui.Models;

namespace WorkoutSample.Maui.Services;

public class AppService : IAppService
{
    private readonly HttpClient _httpClient;

    public AppService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> RefreshToken()
    {
        var isTokenRefreshed = false;
        var url = $"{Settings.BaseUrl}{ApiEndpoints.UsersRefreshToken}";
        var refreshTokenRequest = new RefreshRequest
        {
            RefreshToken = Settings.UserBasicDetails.RefreshToken
        };
        var serializedString = JsonSerializer.Serialize(refreshTokenRequest);

        var response = await _httpClient.PostAsync(url,
            new StringContent(serializedString, Encoding.UTF8, "application/json"));
        if (response.IsSuccessStatusCode)
        {
            var contentStr = await response.Content.ReadAsStringAsync();
            var accessTokenResponse = JsonSerializer.Deserialize<AccessTokenResponse>(contentStr)!;
            Settings.UserBasicDetails.AccessToken = accessTokenResponse.AccessToken;
            Settings.UserBasicDetails.RefreshToken = accessTokenResponse.RefreshToken;

            var userDetailsString = JsonSerializer.Serialize(Settings.UserBasicDetails);
            await SecureStorage.SetAsync(nameof(Settings.UserBasicDetails), userDetailsString);
            isTokenRefreshed = true;
        }

        return isTokenRefreshed;
    }

    public async Task<AccessTokenResponse?> AuthenticateUser(LoginModel loginModel)
    {
        var url = $"{Settings.BaseUrl}{ApiEndpoints.UsersLogin}";
        var serializedString = JsonSerializer.Serialize(loginModel);
        var response =
            await _httpClient.PostAsync(url, new StringContent(serializedString, Encoding.UTF8, "application/json"));
        if (!response.IsSuccessStatusCode) return null;

        var contentString = await response.Content.ReadAsStringAsync();
        var returnResponse = JsonSerializer.Deserialize<AccessTokenResponse>(contentString)!;

        return returnResponse;
    }

    public async Task<(bool IsSuccess, string ErrorMessage)> RegisterUser(RegistrationModel registerUser)
    {
        var errorMessage = string.Empty;
        var isSuccess = false;

        var url = $"{Settings.BaseUrl}{ApiEndpoints.UsersRegister}";
        var serializedString = JsonSerializer.Serialize(registerUser);
        var response =
            await _httpClient.PostAsync(url, new StringContent(serializedString, Encoding.UTF8, "application/json"));
        if (response.IsSuccessStatusCode)
        {
            isSuccess = true;
        }
        else
        {
            errorMessage = await response.Content.ReadAsStringAsync();
        }

        return (isSuccess, errorMessage);
    }

    public Task<List<WorkoutModel>> GetWorkouts(DateOnly workoutDate)
    {
        throw new NotImplementedException();
    }
}