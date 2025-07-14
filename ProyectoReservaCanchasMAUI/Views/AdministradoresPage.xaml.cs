using ProyectoReservaCanchasMAUI.ViewModels;

namespace ProyectoReservaCanchasMAUI.Views;

public partial class AdministradoresPage : ContentPage
{
    // ViewModel para la página de administradores
    private readonly AdministradorViewModel _viewModel;
  

    public AdministradoresPage()
    {
        InitializeComponent();
        // Obtener el ViewModel usando inyección de dependencias
        _viewModel = App.Current?.Handler?.MauiContext?.Services.GetService<AdministradorViewModel>()
                     ?? throw new Exception("AdministradorViewModel no pudo ser resuelto");
        BindingContext = _viewModel;
        // Ejecutar el comando de carga inicial
        _viewModel.CargarCommand.Execute(null);
    }
}


