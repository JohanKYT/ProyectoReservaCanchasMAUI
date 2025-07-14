using ProyectoReservaCanchasMAUI.Models;
using ProyectoReservaCanchasMAUI.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ProyectoReservaCanchasMAUI.ViewModels
{
    public class CarreraViewModel : BaseViewModel
    {
        private readonly CarreraService _carreraService;
        private readonly FacultadService _facultadService;

        public ObservableCollection<Carrera> ListaCarreras { get; } = new();
        public ObservableCollection<Facultad> ListaFacultades { get; } = new();

        private Carrera _nuevaCarrera = new();
        public Carrera NuevaCarrera
        {
            get => _nuevaCarrera;
            set { _nuevaCarrera = value; OnPropertyChanged(); }
        }

        private Carrera _carreraSeleccionada;
        public Carrera CarreraSeleccionada
        {
            get => _carreraSeleccionada;
            set
            {
                _carreraSeleccionada = value;
                OnPropertyChanged();

                if (_carreraSeleccionada != null)
                {
                    NuevaCarrera = new Carrera
                    {
                        CarreraId = _carreraSeleccionada.CarreraId,
                        Nombre = _carreraSeleccionada.Nombre,
                        FacultadId = _carreraSeleccionada.FacultadId,
                        Sincronizado = _carreraSeleccionada.Sincronizado,
                        NombreFacultad = _carreraSeleccionada.NombreFacultad
                    };

                    SelectedFacultad = ListaFacultades.FirstOrDefault(f => f.FacultadId == _carreraSeleccionada.FacultadId);
                }
                else
                {
                    SelectedFacultad = null;
                }
            }
        }

        private Facultad _selectedFacultad;
        public Facultad SelectedFacultad
        {
            get => _selectedFacultad;
            set
            {
                _selectedFacultad = value;
                OnPropertyChanged();

                if (_selectedFacultad != null)
                    NuevaCarrera.FacultadId = _selectedFacultad.FacultadId;
            }
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                OnPropertyChanged();
                UpdateCommandsCanExecute();
            }
        }

        public ICommand CargarCommand { get; }
        public ICommand GuardarCommand { get; }
        public ICommand EliminarCommand { get; }

        public CarreraViewModel(CarreraService carreraService, FacultadService facultadService)
        {
            _carreraService = carreraService;
            _facultadService = facultadService;

            CargarCommand = new Command(async () => await CargarAsync(), () => !IsBusy);
            GuardarCommand = new Command(async () => await GuardarAsync(), () => !IsBusy);
            EliminarCommand = new Command(async () => await EliminarAsync(), () => !IsBusy);
        }

        private void UpdateCommandsCanExecute()
        {
            ((Command)CargarCommand).ChangeCanExecute();
            ((Command)GuardarCommand).ChangeCanExecute();
            ((Command)EliminarCommand).ChangeCanExecute();
        }

        public async Task CargarAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                ListaCarreras.Clear();
                ListaFacultades.Clear();

                await _carreraService.SincronizarLocalesConApiAsync();
                await _carreraService.SincronizarDesdeApiAsync();

                var carreras = await _carreraService.ObtenerCarrerasLocalAsync();
                var facultades = await _facultadService.ObtenerFacultadesLocalAsync();

                foreach (var carrera in carreras)
                {
                    var facultadRelacionada = facultades.FirstOrDefault(f => f.FacultadId == carrera.FacultadId);
                    carrera.NombreFacultad = facultadRelacionada?.Nombre ?? "Facultad desconocida";
                    ListaCarreras.Add(carrera);
                }

                foreach (var f in facultades)
                    ListaFacultades.Add(f);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task GuardarAsync()
        {
            if (IsBusy) return;

            if (string.IsNullOrWhiteSpace(NuevaCarrera.Nombre))
            {
                await App.Current.MainPage.DisplayAlert("Error", "Debe ingresar el nombre de la carrera.", "OK");
                return;
            }

            if (SelectedFacultad == null)
            {
                await App.Current.MainPage.DisplayAlert("Error", "Debe seleccionar una facultad.", "OK");
                return;
            }

            try
            {
                IsBusy = true;
                UpdateCommandsCanExecute();

                NuevaCarrera.FacultadId = SelectedFacultad.FacultadId;

                await _carreraService.GuardarCarreraTotalAsync(NuevaCarrera);
                await _carreraService.SincronizarLocalesConApiAsync();

                var listaActualizada = await _carreraService.ObtenerCarrerasLocalAsync();

                ListaCarreras.Clear();

                var facultades = await _facultadService.ObtenerFacultadesLocalAsync();

                foreach (var c in listaActualizada)
                {
                    var facultadRelacionado = facultades.FirstOrDefault(f => f.FacultadId == c.FacultadId);
                    c.NombreFacultad = facultadRelacionado?.Nombre ?? "Facultad desconocida";
                    ListaCarreras.Add(c);
                }

                NuevaCarrera = new Carrera();
                CarreraSeleccionada = null;
                SelectedFacultad = null;
            }
            finally
            {
                IsBusy = false;
                UpdateCommandsCanExecute();
            }
        }

        private async Task EliminarAsync()
        {
            if (CarreraSeleccionada == null)
            {
                await App.Current.MainPage.DisplayAlert("Error", "Debe seleccionar una carrera para eliminar.", "OK");
                return;
            }

            try
            {
                await _carreraService.EliminarTotalAsync(CarreraSeleccionada);
                ListaCarreras.Remove(CarreraSeleccionada);
                CarreraSeleccionada = null;
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("No se puede eliminar", ex.Message, "OK");
            }
        }
    }
}
