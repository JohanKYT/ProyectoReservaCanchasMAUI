    using ProyectoReservaCanchasMAUI.Models;
    using ProyectoReservaCanchasMAUI.Services;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Input;

    namespace ProyectoReservaCanchasMAUI.ViewModels
    {
    
        public class AdministradorViewModel : BaseViewModel
        {
            private readonly AdministradorService _service;
            private readonly FacultadService _facultadService;

            public ObservableCollection<Administrador> ListaAdministradores { get; } = new();
            public ObservableCollection<Facultad> ListaFacultades { get; } = new();

            private Administrador _nuevoAdministrador = new();
            public Administrador NuevoAdministrador
            {
                get => _nuevoAdministrador;
                set { _nuevoAdministrador = value; OnPropertyChanged(); }
            }

            private Administrador _administradorSeleccionado;
            public Administrador AdministradorSeleccionado
            {
                get => _administradorSeleccionado;
                set
                {
                    _administradorSeleccionado = value;
                    OnPropertyChanged();

                    if (_administradorSeleccionado != null)
                    {
                        NuevoAdministrador = new Administrador
                        {
                            BannerId = _administradorSeleccionado.BannerId,
                            Nombre = _administradorSeleccionado.Nombre,
                            Correo = _administradorSeleccionado.Correo,
                            Password = _administradorSeleccionado.Password,
                            Telefono = _administradorSeleccionado.Telefono,
                            Direccion = _administradorSeleccionado.Direccion,
                            FechaNacimiento = _administradorSeleccionado.FechaNacimiento,
                            FacultadId = _administradorSeleccionado.FacultadId,
                            Sincronizado = _administradorSeleccionado.Sincronizado
                        };
                        FacultadSeleccionada = ListaFacultades.FirstOrDefault(f => f.FacultadId == NuevoAdministrador.FacultadId);
                    }
                }
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
                        NuevoAdministrador.FacultadId = _facultadSeleccionada.FacultadId;
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

            public AdministradorViewModel(AdministradorService service, FacultadService facultadService)
            {
                _service = service;
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
                    ListaAdministradores.Clear();
                    ListaFacultades.Clear();

                    // Cargar Facultades para el picker
                    var facultades = await _facultadService.ObtenerFacultadesLocalAsync();
                    foreach (var f in facultades)
                    {
                        ListaFacultades.Add(f);
                    }

                    // Sincronizar Administradores local-API
                    await _service.SincronizarLocalesConApiAsync();
                    await _service.SincronizarDesdeApiAsync();

                    var lista = await _service.ObtenerAdministradoresLocalAsync();
                    foreach (var admin in lista)
                    {
                        ListaAdministradores.Add(admin);
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

                if (string.IsNullOrWhiteSpace(NuevoAdministrador.Nombre))
                {
                    await App.Current.MainPage.DisplayAlert("Error", "Debe ingresar el nombre del administrador.", "OK");
                    return;
                }

                if (NuevoAdministrador.FacultadId == 0)
                {
                    await App.Current.MainPage.DisplayAlert("Error", "Debe seleccionar una facultad.", "OK");
                    return;
                }

                try
                {
                    IsBusy = true;
                    UpdateCommandsCanExecute();

                    await _service.GuardarAdministradorTotalAsync(NuevoAdministrador);
                    await _service.SincronizarLocalesConApiAsync();

                    var listaActualizada = await _service.ObtenerAdministradoresLocalAsync();
                    ListaAdministradores.Clear();
                    foreach (var a in listaActualizada)
                        ListaAdministradores.Add(a);

                    NuevoAdministrador = new Administrador();
                    AdministradorSeleccionado = null;
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
                if (AdministradorSeleccionado == null) return;

                try
                {
                    IsBusy = true;

                    try
                    {
                        await _service.EliminarTotalAsync(AdministradorSeleccionado);
                        ListaAdministradores.Remove(AdministradorSeleccionado);

                        NuevoAdministrador = new Administrador();
                        AdministradorSeleccionado = null;
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
