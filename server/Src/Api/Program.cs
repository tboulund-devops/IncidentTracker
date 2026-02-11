using Api.Config;

namespace Api;

public static class Program
{
    private const string AppSettingsPath = "Config/Json";

    private static WebApplication BuildApp()
    {
        Console.WriteLine("TEST");
        var builder = WebApplication.CreateBuilder();
        
        // Load appsettings from Config/Json since they're not in the default location
        builder.Configuration
            .AddJsonFile($"{AppSettingsPath}/appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"{AppSettingsPath}/appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();
        
        var appSettings = AppSettingsFactory.Create(builder.Configuration);
        var serviceManager = new ServiceManager(builder.Services, appSettings, builder.Environment);
        serviceManager.ConfigureAndInitializeServices();
        
        Console.WriteLine("Build complete.");
        return builder.Build();
    }
    public static void Main(string[] args)
    {
        var app = BuildApp();
        
        // Development-only middleware
        if (app.Environment.IsDevelopment())
        {
            Console.WriteLine("✓ Running in Development mode");
            app.UseOpenApi();
            app.UseSwaggerUi();
        }
        else if(app.Environment.IsProduction())
        {
            Console.WriteLine("✓ Running in Production mode");
        }
        else if(app.Environment.IsStaging())
        {
            Console.WriteLine("✓ Running in Staging mode");
        }
        
        // Configure middleware pipeline
        app.UseCors("AllowFrontend");
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        
        app.Run();
    }
    
}