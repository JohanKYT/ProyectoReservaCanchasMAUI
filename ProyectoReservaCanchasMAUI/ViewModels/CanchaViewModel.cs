using ProyectoReservaCanchasMAUI.Models;
using ProyectoReservaCanchasMAUI.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ProyectoReservaCanchasMAUI.ViewModels
{
    public class CanchaViewModel : BaseViewModel
    {
        private readonly CanchaService _canchaService;
        private readonly CampusService _campusService;

        public ObservableCollection<Cancha> ListaCanchas { get; } = new();
        public ObservableCollection<Campus> ListaCampus { get; } = new();

        private Cancha _nuevaCancha = new();
        public Cancha NuevaCancha
        {
            get => _nuevaCancha;
            set { _nuevaCancha = value; OnPropertyChanged(); }
        }

        private Cancha _canchaSeleccionada;
        public Cancha CanchaSeleccionada
        {
            get => _canchaSeleccionada;
            set
            {
                _canchaSeleccionada = value;
                OnPropertyChanged();

                if (_canchaSeleccionada != null)
                {
                    NuevaCancha = new Cancha
                    {
                        CanchaId = _canchaSeleccionada.CanchaId,
                        Nombre = _canchaSeleccionada.Nombre,
                        Tipo = _canchaSeleccionada.Tipo,
                        Disponible = _canchaSeleccionada.Disponible,
                        CampusId = _canchaSeleccionada.CampusId,
                        Sincronizado = _canchaSeleccionada.Sincronizado,
                        NombreCampus = _canchaSeleccionada.NombreCampus
                    };

                    SelectedCampus = ListaCampus.FirstOrDefault(c => c.CampusId == _canchaSeleccionada.CampusId);
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
                    NuevaCancha.CampusId = _selectedCampus.CampusId;
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

        public CanchaViewModel(CanchaService canchaService, CampusService campusService)
        {
            _canchaService = canchaService;
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

                ListaCanchas.Clear();
                ListaCampus.Clear();

                // Sincronizar datos desde API y subir locales pendientes
                await _canchaService.SincronizarLocalesConApiAsync();
                await _canchaService.SincronizarDesdeApiAsync();

                // Cargar datos locales
                var canchas = await _canchaService.ObtenerCanchasLocalAsync();
                var campus = await _campusService.ObtenerCampusLocalAsync();

                // Asignar nombre de campus a cada cancha
                foreach (var cancha in canchas)
                {
                    var campusRelacionado = campus.FirstOrDefault(c => c.CampusId == cancha.CampusId);
                    cancha.NombreCampus = campusRelacionado?.Nombre ?? "Campus desconocido";
                    ListaCanchas.Add(cancha);
                }

                // Agregar campus a la lista para el picker
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

            if (string.IsNullOrWhiteSpace(NuevaCancha.Nombre))
            {
                await App.Current.MainPage.DisplayAlert("Error", "Debe ingresar el nombre de la cancha.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(NuevaCancha.Tipo))
            {
                await App.Current.MainPage.DisplayAlert("Error", "Debe ingresar el tipo de cancha.", "OK");
                return;
            }

            if (SelectedCampus == null)
            {
                await App.Current.MainPage.DisplayAlert("Error", "Debe seleccionar un campus.", "OK");
                return;
            }

            try
            {
                IsBusy = true;
                UpdateCommandsCanExecute();

                NuevaCancha.CampusId = SelectedCampus.CampusId;

                await _canchaService.GuardarCanchaTotalAsync(NuevaCancha);
                await _canchaService.SincronizarLocalesConApiAsync();

                var listaActualizada = await _canchaService.ObtenerCanchasLocalAsync();
                var campus = await _campusService.ObtenerCampusLocalAsync();

                ListaCanchas.Clear();

                foreach (var cancha in listaActualizada)
                {
                    var campusRelacionado = campus.FirstOrDefault(c => c.CampusId == cancha.CampusId);
                    cancha.NombreCampus = campusRelacionado?.Nombre ?? "Campus desconocido";
                    ListaCanchas.Add(cancha);
                }

                NuevaCancha = new Cancha();
                CanchaSeleccionada = null;
                SelectedCampus = null;
            }
            finally
            {
                IsBusy = false;
                UpdateCommandsCanExecute();
            }
        }

        private async Task EliminarAsync()
        {
            if (CanchaSeleccionada == null)
            {
                await App.Current.MainPage.DisplayAlert("Error", "Debe seleccionar una cancha para eliminar.", "OK");
                return;
            }

            try
            {
                await _canchaService.EliminarTotalAsync(CanchaSeleccionada);
                ListaCanchas.Remove(CanchaSeleccionada);
                CanchaSeleccionada = null;
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("No se puede eliminar", ex.Message, "OK");
            }
        }
    }
}
