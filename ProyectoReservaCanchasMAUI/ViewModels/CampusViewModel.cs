using ProyectoReservaCanchasMAUI.Models;
using ProyectoReservaCanchasMAUI.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;



namespace ProyectoReservaCanchasMAUI.ViewModels
{
    public class CampusViewModel : BaseViewModel
    {
        private readonly CampusService _service;

        public ObservableCollection<Campus> ListaCampus { get; set; } = new();

        public ICommand CargarCommand { get; }
        public ICommand GuardarCommand { get; }
        public ICommand EliminarCommand { get; }

        private Campus _nuevoCampus = new();
        public Campus NuevoCampus
        {
            get => _nuevoCampus;
            set
            {
                _nuevoCampus = value;
                OnPropertyChanged();
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
                if (_campusSeleccionado != null)
                {
                    NuevoCampus = new Campus
                    {
                        CampusId = _campusSeleccionado.CampusId,
                        Nombre = _campusSeleccionado.Nombre,
                        Direccion = _campusSeleccionado.Direccion
                    };
                }
            }
        }

        public CampusViewModel(CampusService service)
        {
            _service = service;
            CargarCommand = new Command(async () => await CargarCampus());
            GuardarCommand = new Command(async () => await GuardarCampus());
            EliminarCommand = new Command(async () => await EliminarCampus());
        }

        private async Task CargarCampus()
        {
            // Sincronizar locales primero (SQLite → API)
            await _service.SincronizarLocalesConApiAsync();

            // Sincronizar desde API (API → SQLite)
            await _service.SincronizarDesdeApiAsync();

            ListaCampus.Clear();
            var lista = await _service.ObtenerCampusLocalAsync();
            foreach (var item in lista) ListaCampus.Add(item);
        }

        private async Task GuardarCampus()
        {
            await _service.GuardarCampusTotalAsync(NuevoCampus);
            await CargarCampus();
            NuevoCampus = new Campus();
        }
        
        private async Task EliminarCampus()
        {
            if (CampusSeleccionado != null)
            {
                await _service.EliminarCampusAsync(CampusSeleccionado);
                await CargarCampus();
                NuevoCampus = new Campus();
                CampusSeleccionado = null;
            }
        }
    }
}
