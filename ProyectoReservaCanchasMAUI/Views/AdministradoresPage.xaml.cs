using ProyectoReservaCanchasMAUI.Models;
using ProyectoReservaCanchasMAUI.Services;

namespace ProyectoReservaCanchasMAUI.Views;

public partial class AdministradoresPage : ContentPage
{
    private readonly AdministradorService _service;
    private Administrador _adminSeleccionado;

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

    private async void OnAgregarClicked(object sender, EventArgs e)
    {
        var nuevoAdmin = new Administrador
        {
            Nombre = entryNombre.Text,
            Correo = entryCorreo.Text,
            Password = entryPassword.Text,
            Telefono = entryTelefono.Text,
            Direccion = entryDireccion.Text,
            FechaNacimiento = pickerFechaNacimiento.Date,
            TipoPersona = "Administrador",
            FacultadId = int.TryParse(entryFacultadId.Text, out int facId) ? facId : 0
        };

        bool exito = await _service.AgregarAdministradorAsync(nuevoAdmin);
        if (exito)
        {
            await DisplayAlert("Éxito", "Administrador agregado correctamente", "OK");
            CargarDatos();
            LimpiarFormulario();
        }
        else
        {
            await DisplayAlert("Error", "No se pudo agregar el administrador", "OK");
        }
    }

    private void OnSeleccionarAdmin(object sender, SelectionChangedEventArgs e)
    {
        _adminSeleccionado = e.CurrentSelection.FirstOrDefault() as Administrador;
        if (_adminSeleccionado != null)
        {
            entryNombre.Text = _adminSeleccionado.Nombre;
            entryCorreo.Text = _adminSeleccionado.Correo;
            entryPassword.Text = _adminSeleccionado.Password;
            entryTelefono.Text = _adminSeleccionado.Telefono;
            entryDireccion.Text = _adminSeleccionado.Direccion;
            pickerFechaNacimiento.Date = _adminSeleccionado.FechaNacimiento;
            entryFacultadId.Text = _adminSeleccionado.FacultadId.ToString();
        }
    }

    private async void OnActualizarClicked(object sender, EventArgs e)
    {
        if (_adminSeleccionado == null)
        {
            await DisplayAlert("Aviso", "Selecciona un administrador para actualizar", "OK");
            return;
        }

        _adminSeleccionado.Nombre = entryNombre.Text;
        _adminSeleccionado.Correo = entryCorreo.Text;
        _adminSeleccionado.Password = entryPassword.Text;
        _adminSeleccionado.Telefono = entryTelefono.Text;
        _adminSeleccionado.Direccion = entryDireccion.Text;
        _adminSeleccionado.FechaNacimiento = pickerFechaNacimiento.Date;
        _adminSeleccionado.FacultadId = int.TryParse(entryFacultadId.Text, out int facId) ? facId : 0;

        bool exito = await _service.ActualizarAdministradorAsync(_adminSeleccionado);
        if (exito)
        {
            await DisplayAlert("Éxito", "Administrador actualizado correctamente", "OK");
            CargarDatos();
            LimpiarFormulario();
        }
        else
        {
            await DisplayAlert("Error", "No se pudo actualizar el administrador", "OK");
        }
    }

    private async void OnEliminarClicked(object sender, EventArgs e)
    {
        if (_adminSeleccionado == null)
        {
            await DisplayAlert("Aviso", "Selecciona un administrador para eliminar", "OK");
            return;
        }

        bool confirm = await DisplayAlert("Confirmar", $"¿Seguro que quieres eliminar a {_adminSeleccionado.Nombre}?", "Sí", "No");
        if (!confirm) return;

        bool exito = await _service.EliminarAdministradorAsync(_adminSeleccionado.BannerId);
        if (exito)
        {
            await DisplayAlert("Éxito", "Administrador eliminado correctamente", "OK");
            CargarDatos();
            LimpiarFormulario();
        }
        else
        {
            await DisplayAlert("Error", "No se pudo eliminar el administrador", "OK");
        }
    }

    private void LimpiarFormulario()
    {
        entryNombre.Text = string.Empty;
        entryCorreo.Text = string.Empty;
        entryPassword.Text = string.Empty;
        entryTelefono.Text = string.Empty;
        entryDireccion.Text = string.Empty;
        pickerFechaNacimiento.Date = DateTime.Today;
        entryFacultadId.Text = string.Empty;
        _adminSeleccionado = null;
        AdministradoresList.SelectedItem = null;
    }
    private bool isPasswordVisible = false;

    private void OnTogglePasswordVisibilityClicked(object sender, EventArgs e)
    {
        isPasswordVisible = !isPasswordVisible;
        entryPassword.IsPassword = !isPasswordVisible;
        btnTogglePassword.Source = isPasswordVisible ? "eye.png" : "eye_off.png";
    }
}


