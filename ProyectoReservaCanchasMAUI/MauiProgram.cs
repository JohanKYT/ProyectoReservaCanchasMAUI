using Microsoft.Extensions.Logging;
using ProyectoReservaCanchasMAUI.Data;
using ProyectoReservaCanchasMAUI.Services;
using ProyectoReservaCanchasMAUI.ViewModels;
using ProyectoReservaCanchasMAUI.Views;


namespace ProyectoReservaCanchasMAUI
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder.Services.AddSingleton(new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7004/") // Cambiar a IP si usas móvil físico
            });

            // Base de datos local
            builder.Services.AddSingleton<AppDatabase>(s =>
            {
                var dbPath = Path.Combine(FileSystem.AppDataDirectory, "reserva_canchas.db");
                return new AppDatabase(dbPath);
            });

            // Servicios
            builder.Services.AddSingleton<AdministradorService>();
            builder.Services.AddSingleton<FacultadService>();
            builder.Services.AddSingleton<CampusService>();
            builder.Services.AddSingleton<CanchaService>();

            // ViewModels y Vistas
            builder.Services.AddSingleton<CampusViewModel>();
            builder.Services.AddTransient<CampusPage>();

            builder.Services.AddSingleton<FacultadViewModel>();
            builder.Services.AddTransient<FacultadPage>();

            builder.Services.AddTransient<AdministradorViewModel>();
            builder.Services.AddTransient<AdministradoresPage>();

            builder.Services.AddTransient<CanchaViewModel>();
            builder.Services.AddTransient<CanchaPage>();


            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });


#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
