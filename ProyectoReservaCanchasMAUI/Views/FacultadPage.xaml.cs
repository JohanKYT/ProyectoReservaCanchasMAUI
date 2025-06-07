using ProyectoReservaCanchasMAUI.Models;
using ProyectoReservaCanchasMAUI.DTOs;
using ProyectoReservaCanchasMAUI.Services;

namespace ProyectoReservaCanchasMAUI.Views;

public partial class FacultadPage : ContentPage
{
    private readonly FacultadService _service;
    private FacultadDTO _facultadSeleccionada;

    public FacultadPage()
    {
        InitializeComponent();
        _service = new FacultadService();
        CargarDatos();
    }

    private async void CargarDatos()
    {
        var lista = await _service.ObtenerFacultadesAsync();
        FacultadesList.ItemsSource = lista;
    }

    private async void OnAgregarClicked(object sender, EventArgs e)
    {
        var nuevaFacultad = new FacultadDTO
        {
            Nombre = entryNombre.Text,
            CampusId = int.TryParse(entryCampusId.Text, out int campusId) ? campusId : 0
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
            entryCampusId.Text = _facultadSeleccionada.CampusId.ToString();
        }
    }

    private async void OnActualizarClicked(object sender, EventArgs e)
    {
        if (_facultadSeleccionada == null)
        {
            await DisplayAlert("Aviso", "Selecciona una facultad para actualizar", "OK");
            return;
        }

        _facultadSeleccionada.Nombre = entryNombre.Text;
        _facultadSeleccionada.CampusId = int.TryParse(entryCampusId.Text, out int campusId) ? campusId : 0;

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
            await DisplayAlert("Aviso", "Selecciona una facultad para eliminar", "OK");
            return;
        }

        bool confirm = await DisplayAlert("Confirmar", $"¿Seguro que quieres eliminar a {_facultadSeleccionada.Nombre}?", "Sí", "No");
        if (!confirm) return;

        bool exito = await _service.EliminarFacultadAsync(_facultadSeleccionada.FacultadId);
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
        entryCampusId.Text = string.Empty;
        _facultadSeleccionada = null;
        FacultadesList.SelectedItem = null;
    }
}
