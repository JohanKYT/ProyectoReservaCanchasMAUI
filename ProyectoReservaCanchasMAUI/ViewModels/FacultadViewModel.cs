using ProyectoReservaCanchasMAUI.Auxiliares;
using ProyectoReservaCanchasMAUI.Models;
using ProyectoReservaCanchasMAUI.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ProyectoReservaCanchasMAUI.ViewModels
{
    public class FacultadViewModel : BaseViewModel
    {
        private readonly FacultadService _facultadService;
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
            set
            {
                _facultadSeleccionada = value;
                OnPropertyChanged();

                if (_facultadSeleccionada != null)
                {
                    NuevaFacultad = new Facultad
                    {
                        FacultadId = _facultadSeleccionada.FacultadId,
                        Nombre = _facultadSeleccionada.Nombre,
                        CampusId = _facultadSeleccionada.CampusId,
                        Sincronizado = _facultadSeleccionada.Sincronizado
                    };

                    SelectedCampus = ListaCampus.FirstOrDefault(c => c.CampusId == _facultadSeleccionada.CampusId);
                }
                else
                {
                    SelectedCampus = null;
                }
            }
        }
        private Campus _selectedCampus;
        public Campus SelectedCampus
        {
            get => _selectedCampus;
            set
            {
                _selectedCampus = value;
                OnPropertyChanged();

                if (_selectedCampus != null)
                {
                    NuevaFacultad.CampusId = _selectedCampus.CampusId;
                }
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

        public FacultadViewModel(FacultadService facultadService, CampusService campusService)
        {
            _facultadService = facultadService;
            _campusService = campusService;

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

                    ListaFacultades.Clear();
                    ListaCampus.Clear();

                    // Sincronizar locales con API
                    await _facultadService.SincronizarLocalesConApiAsync();
                    await _facultadService.SincronizarDesdeApiAsync();

                    // Obtener facultades locales
                    var facultades = await _facultadService.ObtenerFacultadesLocalAsync();

                    // Obtener campus locales
                    var campus = await _campusService.ObtenerCampusLocalAsync();

                    // Asignar NombreCampus a cada facultad
                    foreach (var facultad in facultades)
                    {
                        var campusRelacionado = campus.FirstOrDefault(c => c.CampusId == facultad.CampusId);
                        facultad.NombreCampus = campusRelacionado?.Nombre ?? "Campus desconocido";  // Asignar NombreCampus

                        ListaFacultades.Add(facultad);
                    }

                    // Agregar campus a la lista de campus
                    foreach (var c in campus)
                    {
                        ListaCampus.Add(c);
                    }
                }
                finally
                {
                    IsBusy = false;
                }
            }

        private async Task GuardarAsync()
        {
            if (IsBusy) return;

            if (string.IsNullOrWhiteSpace(NuevaFacultad.Nombre))
            {
                await App.Current.MainPage.DisplayAlert("Error", "Debe ingresar el nombre de la facultad.", "OK");
                return;
            }

            if (NuevaFacultad.CampusId == 0)
            {
                await App.Current.MainPage.DisplayAlert("Error", "Debe seleccionar un campus.", "OK");
                return;
            }

            bool esNuevo = NuevaFacultad.FacultadId == 0;

            try
            {
                IsBusy = true;
                UpdateCommandsCanExecute();

                await _facultadService.GuardarFacultadTotalAsync(NuevaFacultad);
                await _facultadService.SincronizarLocalesConApiAsync();
                await Logger.LogAsync("Facultad",
                    esNuevo ? "Crear" : "Editar",
                    $"{(esNuevo ? "Creada" : "Editada")} facultad: {NuevaFacultad.Nombre}");


                var listaActualizada = await _facultadService.ObtenerFacultadesLocalAsync();
                ListaFacultades.Clear();
                foreach (var f in listaActualizada)
                    ListaFacultades.Add(f);

                NuevaFacultad = new Facultad();
                FacultadSeleccionada = null;
            }
            finally
            {
                IsBusy = false;
                UpdateCommandsCanExecute();
            }
        }

        private async Task EliminarAsync()
        {
            if (FacultadSeleccionada == null)
            {
                await App.Current.MainPage.DisplayAlert("Error", "Debe seleccionar una facultad para eliminar.", "OK");
                return;
            }

            try
            {
                await _facultadService.EliminarTotalAsync(FacultadSeleccionada);
                await Logger.LogAsync("Facultad",
                    "Eliminar",
                    $"Facultad eliminada: {FacultadSeleccionada.Nombre}");

                ListaFacultades.Remove(FacultadSeleccionada);
                FacultadSeleccionada = null;
            }
            catch (Exception ex)
            {
                // Si hay una excepción, es porque la facultad tiene dependencias o algún error en la API
                await App.Current.MainPage.DisplayAlert("No se puede eliminar", ex.Message, "OK");
            }
        }

    }
}

