using Microsoft.Extensions.Logging;
using ProyectoReservaCanchasMAUI.Data;
using ProyectoReservaCanchasMAUI.Interfaces;
using ProyectoReservaCanchasMAUI.Services;
using ProyectoReservaCanchasMAUI.ViewModels;

namespace ProyectoReservaCanchasMAUI
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder.Services.AddSingleton(new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7004/")
            });
            builder.Services.AddSingleton<AppDatabase>(s =>
            {
                var dbPath = Path.Combine(FileSystem.AppDataDirectory, "reserva_canchas.db");
                return new AppDatabase(dbPath);
            });

            builder.Services.AddSingleton<AdministradorService>();
            builder.Services.AddSingleton<FacultadService>();
            builder.Services.AddSingleton<CampusService>();
            builder.Services.AddSingleton<CanchaService>();

            builder.Services.AddSingleton<SincronizacionViewModel>();
            builder.Services.AddSingleton<ProyectoReservaCanchasMAUI.Views.SincronizacionPage>();
            builder.Services.AddTransient<AdministradorViewModel>();
            builder.Services.AddTransient<ProyectoReservaCanchasMAUI.Views.AdministradoresPage>();
            builder.Services.AddTransient<CanchaViewModel>();
            builder.Services.AddTransient<ProyectoReservaCanchasMAUI.Views.CanchaPage>();
            builder.Services.AddTransient<CampusViewModel>();
            builder.Services.AddTransient<ProyectoReservaCanchasMAUI.Views.CampusPage>();

            // Registrar ISincronizable
            builder.Services.AddSingleton<ISincronizable, CampusService>();
            builder.Services.AddSingleton<ISincronizable, FacultadService>();
            builder.Services.AddSingleton<ISincronizable, CanchaService>();
            builder.Services.AddSingleton<ISincronizable, AdministradorService>();


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
