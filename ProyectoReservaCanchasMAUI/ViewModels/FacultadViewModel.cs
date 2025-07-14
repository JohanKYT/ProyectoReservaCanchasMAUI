using ProyectoReservaCanchasMAUI.Models;
using ProyectoReservaCanchasMAUI.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.ApplicationModel; // Para MainThread

namespace ProyectoReservaCanchasMAUI.ViewModels
{
    public class FacultadViewModel : BaseViewModel
    {
        private readonly FacultadService _service;
        private readonly CampusService _campusService;

        public ObservableCollection<Facultad> ListaFacultades { get; } = new();
        public ObservableCollection<Campus> ListaCampus { get; } = new();

        private Facultad _nuevaFacultad = new();
        public Facultad NuevaFacultad
        {
            get => _nuevaFacultad;
            set { _nuevaFacultad = value; OnPropertyChanged(); }
        }

        private Facultad _facultadSeleccionada;
        public Facultad FacultadSeleccionada
        {
            get => _facultadSeleccionada;
            set { _facultadSeleccionada = value; OnPropertyChanged(); }
        }

        private Campus _campusSeleccionado;
        public Campus CampusSeleccionado
        {
            get => _campusSeleccionado;
            set
            {
                _campusSeleccionado = value;
                if (value != null)
                    NuevaFacultad.CampusId = value.CampusId;

                OnPropertyChanged();
            }
        }

        public ICommand CargarCommand { get; }
        public ICommand GuardarCommand { get; }
        public ICommand EliminarCommand { get; }

        public FacultadViewModel(FacultadService service, CampusService campusService)
        {
            _service = service;
            _campusService = campusService;

            CargarCommand = new Command(async () => await CargarAsync());
            GuardarCommand = new Command(async () => await GuardarAsync());
            EliminarCommand = new Command(async () => await EliminarAsync());

            // Cargar campus para picker al iniciar
            _ = CargarCampusAsync();
        }

        private async Task CargarAsync()
        {
            ListaFacultades.Clear();

            // Sincronizar primero
            await _service.SincronizarLocalesConApiAsync();
            await _service.SincronizarDesdeApiAsync();

            var lista = await _service.ObtenerFacultadesLocalesAsync();
            foreach (var f in lista)
                ListaFacultades.Add(f);
        }

        private async Task GuardarAsync()
        {
            if (string.IsNullOrWhiteSpace(NuevaFacultad.Nombre) || CampusSeleccionado == null)
            {
                await App.Current.MainPage.DisplayAlert("Error", "Debe ingresar nombre y seleccionar campus", "OK");
                return;
            }

            NuevaFacultad.CampusId = CampusSeleccionado.CampusId;
            await _service.GuardarFacultadTotalAsync(NuevaFacultad);

            NuevaFacultad = new Facultad();
            await CargarAsync();
        }

        private async Task EliminarAsync()
        {
            if (FacultadSeleccionada == null) return;

            await _service.EliminarTotalAsync(FacultadSeleccionada);
            ListaFacultades.Remove(FacultadSeleccionada);
            FacultadSeleccionada = null;
        }

        private async Task CargarCampusAsync()
        {
            ListaCampus.Clear();
            var lista = await _campusService.ObtenerCampusLocalAsync();
            foreach (var c in lista)
                ListaCampus.Add(c);
        }
    }
}
