using ProyectoReservaCanchasMAUI.Data;
using ProyectoReservaCanchasMAUI.Services;
using ProyectoReservaCanchasMAUI.ViewModels;

namespace ProyectoReservaCanchasMAUI
{
    public partial class App : Application
    {
        public App(CampusService campusService, FacultadService facultadService, CanchaService canchaService)
        {
            InitializeComponent();

            // ✅ Importante: suscribirse al evento de conectividad
            Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }

        private async void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            if (e.NetworkAccess == NetworkAccess.Internet)
            {
                // ✅ Obtener el ViewModel desde el contenedor de dependencias
                var sincronizador = App.Current?.Handler?.MauiContext?.Services.GetService<SincronizacionViewModel>();
                if (sincronizador != null)
                {
                    await sincronizador.SincronizarTodoAsync();
                }
            }
        }
    }
}