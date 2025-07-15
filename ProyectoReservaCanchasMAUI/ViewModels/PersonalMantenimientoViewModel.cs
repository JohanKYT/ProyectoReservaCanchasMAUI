using ProyectoReservaCanchasMAUI.Auxiliares;
using ProyectoReservaCanchasMAUI.Models;
using ProyectoReservaCanchasMAUI.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ProyectoReservaCanchasMAUI.ViewModels
{
    public class PersonalMantenimientoViewModel : BaseViewModel
    {
        private readonly PersonalMantenimientoService _service;

        public ObservableCollection<PersonalMantenimiento> ListaPersonal { get; } = new();

        private PersonalMantenimiento _nuevoPersonal = new();
        public PersonalMantenimiento NuevoPersonal
        {
            get => _nuevoPersonal;
            set { _nuevoPersonal = value; OnPropertyChanged(); }
        }

        private PersonalMantenimiento _personalSeleccionado;
        public PersonalMantenimiento PersonalSeleccionado
        {
            get => _personalSeleccionado;
            set
            {
                _personalSeleccionado = value;
                OnPropertyChanged();

                if (_personalSeleccionado != null)
                {
                    NuevoPersonal = new PersonalMantenimiento
                    {
                        BannerId = _personalSeleccionado.BannerId,
                        Nombre = _personalSeleccionado.Nombre,
                        Correo = _personalSeleccionado.Correo,
                        Password = _personalSeleccionado.Password,
                        Telefono = _personalSeleccionado.Telefono,
                        Direccion = _personalSeleccionado.Direccion,
                        FechaNacimiento = _personalSeleccionado.FechaNacimiento,
                        Sincronizado = _personalSeleccionado.Sincronizado
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

        public PersonalMantenimientoViewModel(PersonalMantenimientoService service)
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
                ListaPersonal.Clear();

                await _service.SincronizarLocalesConApiAsync();
                await _service.SincronizarDesdeApiAsync();

                var lista = await _service.ObtenerPersonalMantenimientoLocalAsync();
                foreach (var p in lista)
                {
                    ListaPersonal.Add(p);
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

            if (string.IsNullOrWhiteSpace(NuevoPersonal.Nombre))
            {
                await App.Current.MainPage.DisplayAlert("Error", "Debe ingresar el nombre del personal de mantenimiento.", "OK");
                return;
            }

            bool esNuevo = NuevoPersonal.BannerId == 0;


            try
            {
                IsBusy = true;
                UpdateCommandsCanExecute();

                await _service.GuardarPersonalMantenimientoTotalAsync(NuevoPersonal);
                await _service.SincronizarLocalesConApiAsync();
                await Logger.LogAsync("PersonalMantenimiento",
                    esNuevo ? "Crear" : "Editar",
                    $"{(esNuevo ? "Creado" : "Editado")} personal: {NuevoPersonal.Nombre}");


                var listaActualizada = await _service.ObtenerPersonalMantenimientoLocalAsync();
                ListaPersonal.Clear();
                foreach (var p in listaActualizada)
                    ListaPersonal.Add(p);

                NuevoPersonal = new PersonalMantenimiento();
                PersonalSeleccionado = null;
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
            if (PersonalSeleccionado == null) return;

            try
            {
                IsBusy = true;

                try
                {
                    await _service.EliminarTotalAsync(PersonalSeleccionado);
                    await Logger.LogAsync("PersonalMantenimiento",
                        "Eliminar",
                        $"Personal eliminado: {PersonalSeleccionado.Nombre}"
                    );

                    ListaPersonal.Remove(PersonalSeleccionado);

                    NuevoPersonal = new PersonalMantenimiento();
                    PersonalSeleccionado = null;
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

