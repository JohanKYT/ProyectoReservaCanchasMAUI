using ProyectoReservaCanchasMAUI.Models;
using ProyectoReservaCanchasMAUI.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

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

            CargarCommand = new Command(async () =>
            {
                var listaLocal = await _service.ObtenerFacultadesLocalesAsync();
                ListaFacultades.Clear();
                foreach (var f in listaLocal)
                    ListaFacultades.Add(f);

                _ = Task.Run(async () =>
                {
                    await _service.SincronizarLocalesConApiAsync();
                    await _service.SincronizarDesdeApiAsync();

                    var listaActualizada = await _service.ObtenerFacultadesLocalesAsync();
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        ListaFacultades.Clear();
                        foreach (var f in listaActualizada)
                            ListaFacultades.Add(f);
                    });
                });
            });

            GuardarCommand = new Command(async () =>
            {
                if (string.IsNullOrWhiteSpace(NuevaFacultad?.Nombre) || CampusSeleccionado == null)
                    return;

                NuevaFacultad.CampusId = CampusSeleccionado.CampusId;
                await _service.GuardarFacultadTotalAsync(NuevaFacultad);
                NuevaFacultad = new Facultad();
                CargarCommand.Execute(null);
            });

            EliminarCommand = new Command(async () =>
            {
                if (FacultadSeleccionada == null) return;

                await _service.EliminarTotalAsync(FacultadSeleccionada);
                ListaFacultades.Remove(FacultadSeleccionada);
                FacultadSeleccionada = null;
            });

            // Carga inicial de Campus para Picker
            CargarCampus();
        }

        private async void CargarCampus()
        {
            var lista = await _campusService.ObtenerCampusLocalAsync(); // <-- aquí la corrección
            ListaCampus.Clear();
            foreach (var c in lista)
                ListaCampus.Add(c);
        }
    }
}


