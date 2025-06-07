using ProyectoReservaCanchasMAUI.Models;
using ProyectoReservaCanchasMAUI.Services;
using ProyectoReservaCanchasMAUI.DTOs;

namespace ProyectoReservaCanchasMAUI.Views;

public partial class FacultadesPage : ContentPage
{
    private readonly FacultadService _service;
    private readonly CampusService _campusService;
    private FacultadDTO _facultadSeleccionada;
    private List<CampusDTO> _campusList = new();

    public FacultadesPage()
    {
        InitializeComponent();
        _service = new FacultadService();
        _campusService = new CampusService();
        CargarDatos();
    }

    private async void CargarDatos()
    {
        var facultades = await _service.ObtenerFacultadesAsync();
        FacultadesList.ItemsSource = facultades;

        _campusList = await _campusService.ObtenerCampusAsync();
        pickerCampus.ItemsSource = _campusList;
    }

    private async void OnAgregarClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(entryNombre.Text) || pickerCampus.SelectedItem == null)
        {
            await DisplayAlert("Error", "Ingrese nombre y seleccione un campus", "OK");
            return;
        }

        var nuevaFacultad = new FacultadDTO
        {
            Nombre = entryNombre.Text,
            CampusId = ((CampusDTO)pickerCampus.SelectedItem).Id
        };

        bool exito = await _service.AgregarFacultadAsync(nuevaFacultad);
        if (exito)
        {
            await DisplayAlert("Éxito", "Facultad agregada correctamente", "OK");
            CargarDatos();
            LimpiarFormulario();
        }
        else
        {
            await DisplayAlert("Error", "No se pudo agregar la facultad", "OK");
        }
    }

    private void OnSeleccionarFacultad(object sender, SelectionChangedEventArgs e)
    {
        _facultadSeleccionada = e.CurrentSelection.FirstOrDefault() as FacultadDTO;
        if (_facultadSeleccionada != null)
        {
            entryNombre.Text = _facultadSeleccionada.Nombre;

            // Seleccionar campus en picker
            var campusSeleccionado = _campusList.FirstOrDefault(c => c.Id == _facultadSeleccionada.CampusId);
            if (campusSeleccionado != null)
                pickerCampus.SelectedItem = campusSeleccionado;
        }
    }

    private async void OnActualizarClicked(object sender, EventArgs e)
    {
        if (_facultadSeleccionada == null)
        {
            await DisplayAlert("Aviso", "Seleccione una facultad para actualizar", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(entryNombre.Text) || pickerCampus.SelectedItem == null)
        {
            await DisplayAlert("Error", "Ingrese nombre y seleccione un campus", "OK");
            return;
        }

        _facultadSeleccionada.Nombre = entryNombre.Text;
        _facultadSeleccionada.CampusId = ((CampusDTO)pickerCampus.SelectedItem).Id;

        bool exito = await _service.ActualizarFacultadAsync(_facultadSeleccionada);
        if (exito)
        {
            await DisplayAlert("Éxito", "Facultad actualizada correctamente", "OK");
            CargarDatos();
            LimpiarFormulario();
        }
        else
        {
            await DisplayAlert("Error", "No se pudo actualizar la facultad", "OK");
        }
    }

    private async void OnEliminarClicked(object sender, EventArgs e)
    {
        if (_facultadSeleccionada == null)
        {
            await DisplayAlert("Aviso", "Seleccione una facultad para eliminar", "OK");
            return;
        }

        bool confirm = await DisplayAlert("Confirmar", $"¿Seguro que quieres eliminar la facultad {_facultadSeleccionada.Nombre}?", "Sí", "No");
        if (!confirm) return;

        bool exito = await _service.EliminarFacultadAsync(_facultadSeleccionada.Id);
        if (exito)
        {
            await DisplayAlert("Éxito", "Facultad eliminada correctamente", "OK");
            CargarDatos();
            LimpiarFormulario();
        }
        else
        {
            await DisplayAlert("Error", "No se pudo eliminar la facultad", "OK");
        }
    }

    private void LimpiarFormulario()
    {
        entryNombre.Text = string.Empty;
        pickerCampus.SelectedItem = null;
        _facultadSeleccionada = null;
        FacultadesList.SelectedItem = null;
    }
}
