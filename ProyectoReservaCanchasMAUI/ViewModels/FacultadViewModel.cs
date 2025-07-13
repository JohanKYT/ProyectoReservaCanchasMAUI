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
    public class FacultadViewModel : BaseViewModel
    {
        private readonly FacultadService _service;

        public ObservableCollection<Facultad> ListaFacultades { get; set; } = new();
        public ICommand CargarCommand { get; }
        public ICommand GuardarCommand { get; }
        public ICommand EliminarCommand { get; }

        private Facultad _nueva = new();
        public Facultad NuevaFacultad
        {
            get => _nueva;
            set { _nueva = value; OnPropertyChanged(); }
        }

        private Facultad _seleccionada;
        public Facultad FacultadSeleccionada
        {
            get => _seleccionada;
            set
            {
                _seleccionada = value;
                OnPropertyChanged();
                if (_seleccionada != null)
                    NuevaFacultad = new Facultad { FacultadId = _seleccionada.FacultadId, 
                    Nombre = _seleccionada.Nombre, 
                    CampusId = _seleccionada.CampusId };
            }
        }

        public FacultadViewModel(FacultadService service)
        {
            _service = service;
            CargarCommand = new Command(async () => await Cargar());
            GuardarCommand = new Command(async () => await Guardar());
            EliminarCommand = new Command(async () => await Eliminar());
        }


        private async Task Cargar()
        {
            await _service.SincronizarLocalesConApiAsync();
            await _service.SincronizarDesdeApiAsync();

            ListaFacultades.Clear();
            var lista = await _service.ObtenerFacultadesLocalesAsync();
            foreach (var f in lista) ListaFacultades.Add(f);
        }

        private async Task Guardar()
        {
            await _service.GuardarFacultadTotalAsync(NuevaFacultad);
            await Cargar();
            NuevaFacultad = new Facultad();
        }

        private async Task Eliminar()
        {
            if (FacultadSeleccionada != null)
            {
                await _service.EliminarTotalAsync(FacultadSeleccionada);
                await Cargar();
                NuevaFacultad = new Facultad();
                FacultadSeleccionada = null;
            }
        }
    }
}
