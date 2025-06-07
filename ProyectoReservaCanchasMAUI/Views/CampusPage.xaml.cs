using ProyectoReservaCanchasMAUI.Models;
using ProyectoReservaCanchasMAUI.DTOs;
using ProyectoReservaCanchasMAUI.Services;

namespace ProyectoReservaCanchasMAUI.Views;

public partial class CampusPage : ContentPage
{
    private readonly CampusService _service;
    private CampusDTO _campusSeleccionado;

    public CampusPage()
    {
        InitializeComponent();
        _service = new CampusService();
        CargarDatos();
    }

    private async void CargarDatos()
    {
        var lista = await _service.ObtenerCampusAsync();
        CampusList.ItemsSource = lista;
    }

    private async void OnAgregarClicked(object sender, EventArgs e)
    {
        var nuevoCampus = new CampusDTO
        {
            Nombre = entryNombre.Text,
            Direccion = entryDireccion.Text
        };

        bool exito = await _service.AgregarCampusAsync(nuevoCampus);
        if (exito)
        {
            await DisplayAlert("Éxito", "Campus agregado correctamente", "OK");
            CargarDatos();
            LimpiarFormulario();
        }
        else
        {
            await DisplayAlert("Error", "No se pudo agregar el campus", "OK");
        }
    }

    private void OnSeleccionarCampus(object sender, SelectionChangedEventArgs e)
    {
        _campusSeleccionado = e.CurrentSelection.FirstOrDefault() as CampusDTO;
        if (_campusSeleccionado != null)
        {
            entryNombre.Text = _campusSeleccionado.Nombre;
            entryDireccion.Text = _campusSeleccionado.Direccion;
        }
    }

    private async void OnActualizarClicked(object sender, EventArgs e)
    {
        if (_campusSeleccionado == null)
        {
            await DisplayAlert("Aviso", "Selecciona un campus para actualizar", "OK");
            return;
        }

        _campusSeleccionado.Nombre = entryNombre.Text;
        _campusSeleccionado.Direccion = entryDireccion.Text;

        bool exito = await _service.ActualizarCampusAsync(_campusSeleccionado);
        if (exito)
        {
            await DisplayAlert("Éxito", "Campus actualizado correctamente", "OK");
            CargarDatos();
            LimpiarFormulario();
        }
        else
        {
            await DisplayAlert("Error", "No se pudo actualizar el campus", "OK");
        }
    }

    private async void OnEliminarClicked(object sender, EventArgs e)
    {
        if (_campusSeleccionado == null)
        {
            await DisplayAlert("Aviso", "Selecciona un campus para eliminar", "OK");
            return;
        }

        bool confirm = await DisplayAlert("Confirmar", $"¿Seguro que quieres eliminar a {_campusSeleccionado.Nombre}?", "Sí", "No");
        if (!confirm) return;

        bool exito = await _service.EliminarCampusAsync(_campusSeleccionado.CampusId);
        if (exito)
        {
            await DisplayAlert("Éxito", "Campus eliminado correctamente", "OK");
            CargarDatos();
            LimpiarFormulario();
        }
        else
        {
            await DisplayAlert("Error", "No se pudo eliminar el campus", "OK");
        }
    }

    private void LimpiarFormulario()
    {
        entryNombre.Text = string.Empty;
        entryDireccion.Text = string.Empty;
        _campusSeleccionado = null;
        CampusList.SelectedItem = null;
    }
}
