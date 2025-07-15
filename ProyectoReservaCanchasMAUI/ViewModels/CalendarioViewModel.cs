using ProyectoReservaCanchasMAUI.Auxiliares;
using ProyectoReservaCanchasMAUI.Models;
using ProyectoReservaCanchasMAUI.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using ProyectoReservaCanchasMAUI.Auxiliares;

namespace ProyectoReservaCanchasMAUI.ViewModels
{
    public class CalendarioViewModel : BaseViewModel
    {
        private readonly CalendarioService _calendarioService;
        private readonly CanchaService _canchaService;
        private readonly EstudianteService _estudianteService;
        private readonly AdministradorService _administradorService;
        private readonly PersonalMantenimientoService _personalService;

        public ObservableCollection<Calendario> Calendarios { get; } = new();
        public ObservableCollection<Cancha> ListaCanchas { get; } = new();
        public ObservableCollection<PersonaOpcion> PersonasDisponibles { get; } = new();

        private Calendario _calendarioActual = new();
        public Calendario CalendarioActual
        {
            get => _calendarioActual;
            set => SetProperty(ref _calendarioActual, value);
        }

        private PersonaOpcion _personaSeleccionada;
        public PersonaOpcion PersonaSeleccionada
        {
            get => _personaSeleccionada;
            set
            {
                if (SetProperty(ref _personaSeleccionada, value) && value != null)
                {
                    CalendarioActual.PersonaUdlaId = value.Id;
                }
            }
        }
        private Calendario _calendarioSeleccionado;
        public Calendario CalendarioSeleccionado
        {
            get => _calendarioSeleccionado;
            set
            {
                if (SetProperty(ref _calendarioSeleccionado, value) && value != null)
                {
                    CalendarioActual = new Calendario
                    {
                        CalendarioId = value.CalendarioId,
                        Estado = value.Estado,
                        FechaHoraInicio = value.FechaHoraInicio,
                        FechaHoraFin = value.FechaHoraFin,
                        PersonaUdlaId = value.PersonaUdlaId,
                        CanchaId = value.CanchaId
                    };

                    PersonaSeleccionada = PersonasDisponibles.FirstOrDefault(p => p.Id == value.PersonaUdlaId);
                    CanchaSeleccionada = ListaCanchas.FirstOrDefault(c => c.CanchaId == value.CanchaId);
                }
            }
        }

        private Cancha _canchaSeleccionada;
        public Cancha CanchaSeleccionada
        {
            get => _canchaSeleccionada;
            set
            {
                if (SetProperty(ref _canchaSeleccionada, value) && value != null)
                {
                    CalendarioActual.CanchaId = value.CanchaId;
                }
            }
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                SetProperty(ref _isBusy, value);
                UpdateCommandsCanExecute();
            }
        }

        public ICommand CargarCommand { get; }
        public ICommand GuardarCommand { get; }
        public ICommand EliminarCommand { get; }

        public CalendarioViewModel(
            CalendarioService calendarioService,
            CanchaService canchaService,
            EstudianteService estudianteService,
            AdministradorService administradorService,
            PersonalMantenimientoService personalService)
        {
            _calendarioService = calendarioService;
            _canchaService = canchaService;
            _estudianteService = estudianteService;
            _administradorService = administradorService;
            _personalService = personalService;

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
            IsBusy = true;

            try
            {
                Calendarios.Clear();
                ListaCanchas.Clear();
                PersonasDisponibles.Clear();

                // Cargar datos locales
                var calendarios = await _calendarioService.ObtenerCalendariosLocalAsync();
                foreach (var c in calendarios)
                    Calendarios.Add(c);

                var canchas = await _canchaService.ObtenerCanchasLocalAsync();
                foreach (var cancha in canchas)
                    ListaCanchas.Add(cancha);

                var estudiantes = await _estudianteService.ObtenerEstudiantesLocalAsync();
                var administradores = await _administradorService.ObtenerAdministradoresLocalAsync();
                var personalMantenimiento = await _personalService.ObtenerPersonalMantenimientoLocalAsync();

                // Combinar personas para picker
                foreach (var est in estudiantes)
                    PersonasDisponibles.Add(new PersonaOpcion { Id = est.BannerId, Nombre = est.Nombre, Tipo = "Estudiante" });
                foreach (var adm in administradores)
                    PersonasDisponibles.Add(new PersonaOpcion { Id = adm.BannerId, Nombre = adm.Nombre, Tipo = "Administrador" });
                foreach (var per in personalMantenimiento)
                    PersonasDisponibles.Add(new PersonaOpcion { Id = per.BannerId, Nombre = per.Nombre, Tipo = "PersonalMantenimiento" });

                // Sincronizar con API (opcional)
                await _calendarioService.SincronizarLocalesConApiAsync();
                await _calendarioService.SincronizarDesdeApiAsync();
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine($"Error cargando datos en CalendarioViewModel: {ex.Message}");
                await App.Current.MainPage.DisplayAlert("Error", "No se pudieron cargar los datos. Trabajando en modo offline.", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task GuardarAsync()
        {
            if (IsBusy) return;

            var ahora = DateTime.Now;

            // Validación: inicio antes que fin
            if (CalendarioActual.FechaHoraInicio >= CalendarioActual.FechaHoraFin)
            {
                await App.Current.MainPage.DisplayAlert("Error", "La fecha/hora de inicio debe ser menor que la de fin.", "OK");
                return;
            }

            // Validación: inicio no puede ser en el pasado
            if (CalendarioActual.FechaHoraInicio < ahora)
            {
                await App.Current.MainPage.DisplayAlert("Error", "La fecha/hora de inicio no puede ser en el pasado.", "OK");
                return;
            }

            // Validación: debe reservar con al menos 1 hora de anticipación
            if ((CalendarioActual.FechaHoraInicio - ahora).TotalMinutes < 60)
            {
                await App.Current.MainPage.DisplayAlert("Error", "La reserva debe realizarse con al menos 1 hora de anticipación.", "OK");
                return;
            }

            // Validación: cancha seleccionada
            if (CalendarioActual.CanchaId == 0)
            {
                await App.Current.MainPage.DisplayAlert("Error", "Debe seleccionar una cancha.", "OK");
                return;
            }

            // Validación: persona seleccionada
            if (CalendarioActual.PersonaUdlaId == 0)
            {
                await App.Current.MainPage.DisplayAlert("Error", "Debe seleccionar una persona.", "OK");
                return;
            }

            // Validación conflicto de reservas cancha
            bool existeConflicto = Calendarios.Any(c =>
                c.CanchaId == CalendarioActual.CanchaId &&
                c.CalendarioId != CalendarioActual.CalendarioId && // Para evitar conflicto con sí mismo al editar
                (
                    (CalendarioActual.FechaHoraInicio >= c.FechaHoraInicio && CalendarioActual.FechaHoraInicio < c.FechaHoraFin) ||
                    (CalendarioActual.FechaHoraFin > c.FechaHoraInicio && CalendarioActual.FechaHoraFin <= c.FechaHoraFin) ||
                    (CalendarioActual.FechaHoraInicio <= c.FechaHoraInicio && CalendarioActual.FechaHoraFin >= c.FechaHoraFin)
                )
            );

            if (existeConflicto)
            {
                await App.Current.MainPage.DisplayAlert("Error", "La cancha ya está reservada en el horario seleccionado.", "OK");
                return;
            }

            // Declaramos la variable correctamente
            bool esNuevo = CalendarioActual.CalendarioId == 0;

            try
            {
                IsBusy = true;
                UpdateCommandsCanExecute();

                await _calendarioService.GuardarCalendarioTotalAsync(CalendarioActual);
                await _calendarioService.SincronizarLocalesConApiAsync();
                await Logger.LogAsync("Calendario",esNuevo ? "Crear" : "Editar",$"Reserva {(esNuevo ? "creada" : "actualizada")} para cancha ID {CalendarioActual.CanchaId} desde {CalendarioActual.FechaHoraInicio} hasta {CalendarioActual.FechaHoraFin}");

                // Recarga datos para reflejar cambios
                await CargarAsync();

                // Reset formulario con fechas inicializadas para evitar problemas en DatePicker
                CalendarioActual = new Calendario
                {
                    FechaHoraInicio = DateTime.Now,
                    FechaHoraFin = DateTime.Now.AddHours(1)
                };
                PersonaSeleccionada = null;
                CanchaSeleccionada = null;
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine($"Error guardando calendario: {ex.Message}");
                await App.Current.MainPage.DisplayAlert("Error", "No se pudo guardar la reserva.", "OK");
            }
            finally
            {
                IsBusy = false;
                UpdateCommandsCanExecute();
            }
        }

        private async Task EliminarAsync()
        {
            if (IsBusy || CalendarioSeleccionado == null)
            {
                await App.Current.MainPage.DisplayAlert("Aviso", "Debe seleccionar una reserva para eliminar.", "OK");
                return;
            }

            bool confirm = await App.Current.MainPage.DisplayAlert("Confirmar", "¿Deseas eliminar esta reserva?", "Sí", "No");
            if (!confirm) return;

            try
            {
                IsBusy = true;
                UpdateCommandsCanExecute();

                await _calendarioService.EliminarTotalAsync(CalendarioSeleccionado);
                await _calendarioService.SincronizarLocalesConApiAsync();
                await Logger.LogAsync("Calendario", "Eliminar", $"Reserva eliminada para cancha ID {CalendarioActual.CanchaId} del {CalendarioActual.FechaHoraInicio}");


                await CargarAsync();

                // Reset
                CalendarioActual = new Calendario();
                CalendarioSeleccionado = null;
                PersonaSeleccionada = null;
                CanchaSeleccionada = null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error eliminando calendario: {ex.Message}");
                await App.Current.MainPage.DisplayAlert("Error", "No se pudo eliminar la reserva.", "OK");
            }
            finally
            {
                IsBusy = false;
                UpdateCommandsCanExecute();
            }
        }
    }
}
