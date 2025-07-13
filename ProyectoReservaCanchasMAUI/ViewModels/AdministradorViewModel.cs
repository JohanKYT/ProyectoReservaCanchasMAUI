using ProyectoReservaCanchasMAUI.Models;
using ProyectoReservaCanchasMAUI.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ProyectoReservaCanchasMAUI.ViewModels
{
    public class AdministradorViewModel : BaseViewModel
    {
        private readonly AdministradorService _service;
        private readonly FacultadService _facultadService;

        public ObservableCollection<Administrador> ListaAdministradores { get; set; } = new();
        public Administrador NuevoAdministrador { get; set; } = new();
        public Administrador AdministradorSeleccionado { get; set; }

        public ICommand CargarCommand { get; }
        public ICommand GuardarCommand { get; }
        public ICommand EliminarCommand { get; }

        public AdministradorViewModel(AdministradorService service, FacultadService facultadService)
        {
            _service = service;
            _facultadService = facultadService;
            CargarCommand = new Command(async () => await Cargar());
            GuardarCommand = new Command(async () => await Guardar());
            EliminarCommand = new Command(async () => await Eliminar());
            _facultadService = facultadService;
        }

        private async Task Cargar()
        {
            // Cargar facultades locales para obtener nombres
            var listaFacultades = await _facultadService.ObtenerFacultadesLocalesAsync();
            var facultadDict = listaFacultades.ToDictionary(f => f.FacultadId, f => f.Nombre);

            ListaAdministradores.Clear();
            var datos = await _service.ObtenerAdministradoresLocalesAsync();

            foreach (var admin in datos)
            {
                // Asignar nombre de facultad para mostrar en UI
                admin.NombreFacultad = facultadDict.TryGetValue(admin.FacultadId, out var nombreFacultad)
                    ? nombreFacultad
                    : "Desconocido";

                ListaAdministradores.Add(admin);
            }
        }

        private async Task Guardar()
        {
            if (NuevoAdministrador != null)
            {
                await _service.GuardarTotalAsync(NuevoAdministrador);
                await Cargar();
                NuevoAdministrador = new Administrador();
                OnPropertyChanged(nameof(NuevoAdministrador));
            }
        }

        private async Task Eliminar()
        {
            if (AdministradorSeleccionado != null)
            {
                await _service.EliminarTotalAsync(AdministradorSeleccionado);
                await Cargar();
                AdministradorSeleccionado = null;
                NuevoAdministrador = new Administrador();
                OnPropertyChanged(nameof(NuevoAdministrador));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
