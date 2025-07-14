using ProyectoReservaCanchasMAUI.Models;
using ProyectoReservaCanchasMAUI.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ProyectoReservaCanchasMAUI.ViewModels
{
    public class CampusViewModel : BaseViewModel
    {
        private readonly CampusService _service;

        public ObservableCollection<Campus> ListaCampus { get; } = new();

        private Campus _nuevoCampus = new();
        public Campus NuevoCampus
        {
            get => _nuevoCampus;
            set { _nuevoCampus = value; OnPropertyChanged(); }
        }

        private Campus _campusSeleccionado;
        public Campus CampusSeleccionado
        {
            get => _campusSeleccionado;
            set
            {
                _campusSeleccionado = value;
                OnPropertyChanged();

                if (_campusSeleccionado != null)
                {
                    NuevoCampus = new Campus
                    {
                        CampusId = _campusSeleccionado.CampusId,
                        Nombre = _campusSeleccionado.Nombre,
                        Direccion = _campusSeleccionado.Direccion,
                        Sincronizado = _campusSeleccionado.Sincronizado
                    };
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

        public CampusViewModel(CampusService service)
        {
            _service = service;

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
                ListaCampus.Clear();

                // Primero sube y sincroniza los locales con la API,
                // aquí es donde se actualizan los IDs locales
                await _service.SincronizarLocalesConApiAsync();

                // Luego baja la lista actualizada de la API y reemplaza localmente
                await _service.SincronizarDesdeApiAsync();

                var lista = await _service.ObtenerCampusLocalAsync();

                foreach (var campus in lista)
                {
                    ListaCampus.Add(campus);
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

            if (string.IsNullOrWhiteSpace(NuevoCampus.Nombre))
            {
                await App.Current.MainPage.DisplayAlert("Error", "Debe ingresar el nombre del campus.", "OK");
                return;
            }

            try
            {
                IsBusy = true;
                UpdateCommandsCanExecute();

                // Guardar localmente
                await _service.GuardarCampusTotalAsync(NuevoCampus);

                // Subir al API los datos locales pendientes
                await _service.SincronizarLocalesConApiAsync();

                // Recargar lista local actualizada
                var listaActualizada = await _service.ObtenerCampusLocalAsync();

                ListaCampus.Clear();
                foreach (var c in listaActualizada)
                    ListaCampus.Add(c);

                NuevoCampus = new Campus();
                CampusSeleccionado = null;
            }
            finally
            {
                IsBusy = false;
                UpdateCommandsCanExecute();
            }
        }

        private async Task EliminarAsync()
        {
            if (IsBusy) return;
            if (CampusSeleccionado == null) return;

            try
            {
                IsBusy = true;

                await _service.EliminarTotalAsync(CampusSeleccionado);
                ListaCampus.Remove(CampusSeleccionado);

                NuevoCampus = new Campus();
                CampusSeleccionado = null;
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}