using ProyectoReservaCanchasMAUI.Auxiliares;
using ProyectoReservaCanchasMAUI.Models;
using ProyectoReservaCanchasMAUI.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ProyectoReservaCanchasMAUI.ViewModels
{
    public class EstudianteViewModel : BaseViewModel
    {
        private readonly EstudianteService _service;
        private readonly CarreraService _carreraService;

        public ObservableCollection<Estudiante> ListaEstudiantes { get; } = new();
        public ObservableCollection<Carrera> ListaCarreras { get; } = new();

        private Estudiante _nuevoEstudiante = new();
        public Estudiante NuevoEstudiante
        {
            get => _nuevoEstudiante;
            set { _nuevoEstudiante = value; OnPropertyChanged(); }
        }

        private Estudiante _estudianteSeleccionado;
        public Estudiante EstudianteSeleccionado
        {
            get => _estudianteSeleccionado;
            set
            {
                _estudianteSeleccionado = value;
                OnPropertyChanged();

                if (_estudianteSeleccionado != null)
                {
                    NuevoEstudiante = new Estudiante
                    {
                        BannerId = _estudianteSeleccionado.BannerId,
                        Nombre = _estudianteSeleccionado.Nombre,
                        Correo = _estudianteSeleccionado.Correo,
                        Password = _estudianteSeleccionado.Password,
                        Telefono = _estudianteSeleccionado.Telefono,
                        Direccion = _estudianteSeleccionado.Direccion,
                        FechaNacimiento = _estudianteSeleccionado.FechaNacimiento,
                        CarreraId = _estudianteSeleccionado.CarreraId,
                        Sincronizado = _estudianteSeleccionado.Sincronizado
                    };
                    CarreraSeleccionada = ListaCarreras.FirstOrDefault(c => c.CarreraId == NuevoEstudiante.CarreraId);
                }
            }
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
                    NuevoEstudiante.CarreraId = _carreraSeleccionada.CarreraId;
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

        public EstudianteViewModel(EstudianteService service, CarreraService carreraService)
        {
            _service = service;
            _carreraService = carreraService;

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
                ListaEstudiantes.Clear();
                ListaCarreras.Clear();

                var carreras = await _carreraService.ObtenerCarrerasLocalAsync();
                foreach (var c in carreras)
                {
                    ListaCarreras.Add(c);
                }

                await _service.SincronizarLocalesConApiAsync();
                await _service.SincronizarDesdeApiAsync();

                var lista = await _service.ObtenerEstudiantesLocalAsync();
                foreach (var est in lista)
                {
                    ListaEstudiantes.Add(est);
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

            if (string.IsNullOrWhiteSpace(NuevoEstudiante.Nombre))
            {
                await App.Current.MainPage.DisplayAlert("Error", "Debe ingresar el nombre del estudiante.", "OK");
                return;
            }

            if (NuevoEstudiante.CarreraId == 0)
            {
                await App.Current.MainPage.DisplayAlert("Error", "Debe seleccionar una carrera.", "OK");
                return;
            }

            bool esNuevo = NuevoEstudiante.BannerId == 0;

            try
            {
                IsBusy = true;
                UpdateCommandsCanExecute();

                await _service.GuardarEstudianteTotalAsync(NuevoEstudiante);
                await _service.SincronizarLocalesConApiAsync();
                await Logger.LogAsync("Estudiante",
                    esNuevo ? "Crear" : "Editar",
                    $"{(esNuevo ? "Creado" : "Editado")} estudiante: {NuevoEstudiante.Nombre}");


                var listaActualizada = await _service.ObtenerEstudiantesLocalAsync();
                ListaEstudiantes.Clear();
                foreach (var est in listaActualizada)
                    ListaEstudiantes.Add(est);

                NuevoEstudiante = new Estudiante();
                EstudianteSeleccionado = null;
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
            if (EstudianteSeleccionado == null) return;

            try
            {
                IsBusy = true;

                try
                {
                    await _service.EliminarTotalAsync(EstudianteSeleccionado);
                    await Logger.LogAsync("Estudiante",
                        "Eliminar",
                        $"Estudiante eliminado: {EstudianteSeleccionado.Nombre}");

                    ListaEstudiantes.Remove(EstudianteSeleccionado);

                    NuevoEstudiante = new Estudiante();
                    EstudianteSeleccionado = null;
                }
                catch (InvalidOperationException ex)
                {
                    await App.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
                }
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
