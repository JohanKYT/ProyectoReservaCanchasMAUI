using ProyectoReservaCanchasMAUI.Models;
using ProyectoReservaCanchasMAUI.Services;

namespace ProyectoReservaCanchasMAUI.Views;

public partial class AdministradoresPage : ContentPage
{
    private readonly AdministradorService _service;

    public AdministradoresPage()
    {
        InitializeComponent();
        _service = new AdministradorService();
        CargarDatos();
    }

    private async void CargarDatos()
    {
        var lista = await _service.ObtenerAdministradoresAsync();
        AdministradoresList.ItemsSource = lista;
    }
    private async void OnAdministradorTapped(object sender, EventArgs e)
    {
        // Obtener el BindingContext (el Administrador seleccionado)
        var frame = sender as Frame;
        if (frame == null) return;

        var administrador = frame.BindingContext as ProyectoReservaCanchasMAUI.Models.Administrador;
        if (administrador == null) return;

        // Construir el mensaje con toda la info que quieras mostrar
        string mensaje = $"Nombre: {administrador.Nombre}\n" +
                         $"Correo: {administrador.Correo}\n" +
                         $"Teléfono: {administrador.Telefono}\n" +
                         $"Dirección: {administrador.Direccion}\n" +
                         $"Fecha Nacimiento: {administrador.FechaNacimiento:yyyy-MM-dd}\n" +
                         $"Tipo: {administrador.TipoPersona}\n" +
                         $"Facultad: {administrador.Facultad?.Nombre}\n" +
                         $"Campus: {administrador.Facultad?.Campus?.Nombre}";

        // Mostrar alerta con la info completa
        await DisplayAlert("Detalle Administrador", mensaje, "Cerrar");
    }
    //Se obtuvo este codigo de ChatGPT para poder mostrar los detalles de cada administrador al hacer click en el frame de cada uno de ellos.
}

