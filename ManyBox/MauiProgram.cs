using LiveChartsCore.SkiaSharpView.Maui;
using ManyBox.Services;
using ManyBox.Utils; // <-- Agrega el using para SessionState
using QuestPDF.Infrastructure; // <-- Agrega este using
using ManyBox.Helpers; // <-- Agrega este using
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop; // <-- Agrega este using

namespace ManyBox;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        // Licencia de QuestPDF: Community o Professional
        QuestPDF.Settings.License = LicenseType.Community;

        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseLiveCharts()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        builder.Services.AddMauiBlazorWebView();

        // Agrega el handler de autenticación para enviar el token JWT en cada request
        builder.Services.AddHttpClient("Api", client =>
        {
            client.BaseAddress = new Uri("http://100.64.197.11:5000/");
            client.Timeout = TimeSpan.FromSeconds(30);
        })
        .AddHttpMessageHandler(() => new AuthHeaderHandler());

        // Esta línea permite inyectar HttpClient directamente
        builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("Api"));

        builder.Services.AddSingleton<LocalDatabaseService>();

        // REGISTRO DE SERVICIOS DE CHAT
        builder.Services.AddScoped<ChatApiService>();
        builder.Services.AddScoped<ChatHubService>();
        builder.Services.AddScoped<UserService>();
        builder.Services.AddScoped<BitacoraHubService>(); // <-- Se agregó el registro de BitacoraHubService

        // REGISTRA NotificacionesService y otros servicios
        // builder.Services.AddSingleton<SessionState>(); // Elimina esta línea, SessionState es static y no debe registrarse
        builder.Services.AddScoped<NotificacionesService>();
        // Eliminado EnvioApiService (solo existe en backend)
        // REGISTRO DE SERVICIO DE CLIENTES
        builder.Services.AddScoped<ClienteApiService>();

        // REGISTRO DE SERVICIO DE USUARIOS
        builder.Services.AddScoped<UsuarioApiService>();

        // REGISTRO DE SERVICIO DE SUCURSALES
        builder.Services.AddScoped<SucursalApiService>();

        // Servicio para PDF de paquetes
        builder.Services.AddSingleton<PDFPaquetes>();

        // REGISTRO DE SERVICIO DE GUIAS DE CHOFER
        builder.Services.AddScoped<GuiasChoferApiService>();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
        builder.Logging.SetMinimumLevel(LogLevel.Debug);
#endif

        // Manejo global de excepciones
        var app = builder.Build();

        AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
        {
            var logger = app.Services.GetService<ILogger<App>>();
            logger?.LogError(e.ExceptionObject as Exception, "Unhandled exception occurred");
        };

        TaskScheduler.UnobservedTaskException += (sender, e) =>
        {
            var logger = app.Services.GetService<ILogger<App>>();
            logger?.LogError(e.Exception, "Unobserved task exception occurred");
            e.SetObserved();
        };

        return app;
    }

}
