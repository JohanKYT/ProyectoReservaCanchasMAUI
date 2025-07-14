using ProyectoReservaCanchasMAUI.Models;
using ProyectoReservaCanchasMAUI.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ProyectoReservaCanchasMAUI.ViewModels
{
    public class CanchaViewModel : BaseViewModel
    {
        private readonly CanchaService _service;
        private readonly CampusService _campusService;

        public ObservableCollection<Cancha> ListaCanchas { get; set; } = new();
        public ObservableCollection<Campus> ListaCampus { get; set; } = new();

        public ICommand CargarCommand { get; }
        public ICommand GuardarCommand { get; }
        public ICommand EliminarCommand { get; }

        private Cancha _nueva = new();
        public Cancha NuevaCancha
        {
            get => _nueva;
            set { _nueva = value; OnPropertyChanged(); }
        }

        private Cancha _canchaSeleccionada;
        public Cancha CanchaSeleccionada
        {
            get => _canchaSeleccionada;
            set
            {
                _canchaSeleccionada = value;
                OnPropertyChanged();
                if (value != null)
                {
                    NuevaCancha = new Cancha
                    {
                        CanchaId = value.CanchaId,
                        Nombre = value.Nombre,
                        Tipo = value.Tipo,
                        Disponible = value.Disponible,
                        CampusId = value.CampusId
                    };

                    CampusSeleccionado = ListaCampus.FirstOrDefault(c => c.CampusId == value.CampusId);
                }
            }
        }
        private Campus _campusSeleccionado;
        public Campus CampusSeleccionado
        {
            get => _campusSeleccionado;
            set
            {
                _campusSeleccionado = value;
                OnPropertyChanged();
                if (NuevaCancha != null && _campusSeleccionado != null)
                    NuevaCancha.CampusId = _campusSeleccionado.CampusId;
            }
        }

        public CanchaViewModel(CanchaService service, CampusService campusService)
        {
            _service = service;
            _campusService = campusService;
            CargarCommand = new Command(async () => await Cargar());
            GuardarCommand = new Command(async () => await Guardar());
            EliminarCommand = new Command(async () => await Eliminar());
        }

        private async Task Cargar()
        {
            try
            {
                await _service.SincronizarLocalesConApiAsync();
                await _service.SincronizarDesdeApiAsync();

                var listaCampus = await _campusService.ObtenerCampusLocalAsync();
                var campusDict = listaCampus.ToDictionary(c => c.CampusId, c => c.Nombre);

                ListaCampus.Clear();
                foreach (var campus in listaCampus)
                    ListaCampus.Add(campus);

                ListaCanchas.Clear();
                var lista = await _service.ObtenerCanchasLocalesAsync();
                foreach (var cancha in lista)
                {
                    cancha.NombreCampus = campusDict.TryGetValue(cancha.CampusId, out var nombreCampus)
                        ? nombreCampus
                        : "Desconocido";

                    ListaCanchas.Add(cancha);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar: {ex.Message}");
                // Aquí también podrías usar: await App.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async Task Guardar()
        {
            try
            {
                await _service.GuardarCanchaTotalAsync(NuevaCancha);
                await Cargar();
                NuevaCancha = new();
            }
            catch (Exception ex)
            {
                // Puedes mostrar esto con un DisplayAlert o log
                Console.WriteLine($"Error en Guardar: {ex.Message}");
            }
        }
        private async Task Eliminar()
        {
            if (CanchaSeleccionada != null)
            {
                await _service.EliminarTotalAsync(CanchaSeleccionada);
                await Cargar();
                NuevaCancha = new();
                CanchaSeleccionada = null;
            }
        }
    }
}
