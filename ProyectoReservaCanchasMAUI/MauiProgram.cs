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

            string baseUrl = "https://localhost:7004/"; // O IP local para Android físico

            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "AppDataBase.db");

            // Base
            builder.Services.AddSingleton<AppDatabase>(s => new AppDatabase(dbPath));


            // Servicios que usan HttpClient (API)
            builder.Services.AddHttpClient<CampusService>(client =>
            {
                client.BaseAddress = new Uri(baseUrl);
            });

            builder.Services.AddHttpClient<FacultadService>(client =>
            {
                client.BaseAddress = new Uri(baseUrl);
            });

            builder.Services.AddHttpClient<CanchaService>(client =>
            {
                client.BaseAddress = new Uri(baseUrl);
            });

            builder.Services.AddHttpClient<AdministradorService>(client =>
            {
                client.BaseAddress = new Uri(baseUrl);
            });

            builder.Services.AddHttpClient<CarreraService>(client =>
            {
                client.BaseAddress = new Uri(baseUrl);
            });
            builder.Services.AddHttpClient<EstudianteService>(client =>
            {
                client.BaseAddress = new Uri(baseUrl);
            });




            // ViewModels
            builder.Services.AddTransient<FacultadViewModel>();
            builder.Services.AddTransient<CampusViewModel>();
            builder.Services.AddTransient<CanchaViewModel>();
            builder.Services.AddTransient<AdministradorViewModel>();
            builder.Services.AddTransient<CarreraViewModel>();
            builder.Services.AddTransient<EstudianteViewModel>();

            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            return builder.Build();
        }
    }
}