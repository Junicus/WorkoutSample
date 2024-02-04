using System.Net.Http.Headers;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WorkoutSample.Maui.Contracts;
using WorkoutSample.Maui.HttpClients;
using WorkoutSample.Maui.Options;
using WorkoutSample.Maui.Services;

namespace WorkoutSample.Maui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts => { fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular"); });

        builder.Services.AddMauiBlazorWebView();
        builder.Services.AddHttpClient();
        builder.AddAppSettings();
        builder.Services.Configure<ApiOptions>(builder.Configuration.GetSection(ApiOptions.SectionName));

        builder.Services.AddHttpClient<IWorkoutSampleApiClient, WorkoutSampleApiClient>((provider, client) =>
        {
            var apiOptions = provider.GetRequiredService<IOptions<ApiOptions>>();
            client.BaseAddress = new Uri(apiOptions.Value.BaseAddress);

            var accessToken = SecureStorage.GetAsync("accessToken").Result;
            if (accessToken != null)
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }
        });

        builder.Services.AddScoped<IWorkoutSampleAppService, WorkoutSampleAppService>();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }

    public static void AddAppSettings(this MauiAppBuilder builder)
    {
        using var stream = Assembly
            .GetExecutingAssembly()
            .GetManifestResourceStream("WorkoutSample.Maui.appsettings.json");

        if (stream != null)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonStream(stream)
                .Build();
            builder.Configuration.AddConfiguration(configuration);
        }
    }
}