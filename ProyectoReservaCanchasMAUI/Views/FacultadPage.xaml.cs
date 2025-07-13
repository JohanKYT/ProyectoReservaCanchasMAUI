using ProyectoReservaCanchasMAUI.Services;
using ProyectoReservaCanchasMAUI.Data;
using ProyectoReservaCanchasMAUI.ViewModels;

namespace ProyectoReservaCanchasMAUI.Views;

public partial class FacultadPage : ContentPage
{
    private readonly FacultadViewModel _viewModel;
    public FacultadPage()
    {

        InitializeComponent();
        // Obtener el ViewModel desde DI (servicios registrados)
        _viewModel = App.Current?.Handler?.MauiContext?.Services.GetService<FacultadViewModel>();

        if (_viewModel == null)
            throw new Exception("FacultadViewModel no pudo ser resuelto desde el contenedor");

        BindingContext = _viewModel;

        // Cargar datos al iniciar
        _viewModel.CargarCommand.Execute(null);
    }
    
}
