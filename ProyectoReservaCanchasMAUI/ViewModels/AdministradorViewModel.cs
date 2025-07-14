using ProyectoReservaCanchasMAUI.Models;
using ProyectoReservaCanchasMAUI.Services;
using ProyectoReservaCanchasMAUI.ViewModels;
using System.Collections.ObjectModel;
using System.Windows.Input;

public class AdministradorViewModel : BaseViewModel
{
    private readonly AdministradorService _service;
    private readonly FacultadService _facultadService;

    public ObservableCollection<Administrador> ListaAdministradores { get; set; } = new();
    public Administrador NuevoAdministrador { get; set; } = new();
    public Administrador AdministradorSeleccionado { get; set; }

    public ICommand CargarCommand { get; }
    public ICommand GuardarCommand { get; }
    public ICommand EliminarCommand { get; }

    public AdministradorViewModel(AdministradorService service, FacultadService facultadService)
    {
        _service = service;
        _facultadService = facultadService;

        CargarCommand = new Command(async () =>
        {
            // Descarga y sincroniza desde API
            await _service.SincronizarDesdeApiAsync();

            // Sube datos locales pendientes
            await _service.SincronizarLocalesConApiAsync();

            // Carga datos locales para UI
            var admins = await _service.ObtenerAdministradoresLocalesAsync();

            ListaAdministradores.Clear();
            foreach (var a in admins)
                ListaAdministradores.Add(a);
        });

        GuardarCommand = new Command(async () =>
        {
            if (string.IsNullOrWhiteSpace(NuevoAdministrador.Nombre) ||
                string.IsNullOrWhiteSpace(NuevoAdministrador.Correo) ||
                string.IsNullOrWhiteSpace(NuevoAdministrador.Password))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Completa todos los campos obligatorios", "OK");
                return;
            }

            await _service.GuardarTotalAsync(NuevoAdministrador);
            NuevoAdministrador = new Administrador();
            CargarCommand.Execute(null);
        });

        EliminarCommand = new Command(async () =>
        {
            if (AdministradorSeleccionado != null)
            {
                await _service.EliminarTotalAsync(AdministradorSeleccionado);
                AdministradorSeleccionado = null;
                NuevoAdministrador = new Administrador();
                CargarCommand.Execute(null);
            }
        });
    }
}