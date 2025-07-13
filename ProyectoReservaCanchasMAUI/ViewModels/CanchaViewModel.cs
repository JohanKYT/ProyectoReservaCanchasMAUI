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

        public ObservableCollection<Cancha> ListaCanchas { get; set; } = new();

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
                        Disponible = value.Disponible
                    };
                }
            }
        }

        public CanchaViewModel(CanchaService service)
        {
            _service = service;
            CargarCommand = new Command(async () => await Cargar());
            GuardarCommand = new Command(async () => await Guardar());
            EliminarCommand = new Command(async () => await Eliminar());
        }

        private async Task Cargar()
        {
            await _service.SincronizarDesdeApiAsync();
            ListaCanchas.Clear();
            var lista = await _service.ObtenerCanchasLocalesAsync();
            foreach (var cancha in lista)
                ListaCanchas.Add(cancha);
        }

        private async Task Guardar()
        {
            await _service.GuardarCanchaTotalAsync(NuevaCancha);
            await Cargar();
            NuevaCancha = new();
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
