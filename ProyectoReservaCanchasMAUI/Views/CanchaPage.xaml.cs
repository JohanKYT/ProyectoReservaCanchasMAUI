using ProyectoReservaCanchasMAUI.Services;
using ProyectoReservaCanchasMAUI.ViewModels;

namespace ProyectoReservaCanchasMAUI.Views;

public partial class CanchaPage : ContentPage
{
    private readonly CampusViewModel _viewModel;
    public CanchaPage()
	{
		InitializeComponent();
        // Obtener el ViewModel desde DI (servicios registrados)
        _viewModel = App.Current?.Handler?.MauiContext?.Services.GetService<CampusViewModel>();

        if (_viewModel == null)
            throw new Exception("CampusViewModel no pudo ser resuelto desde el contenedor");

        BindingContext = _viewModel;

        // Cargar datos al iniciar
        _viewModel.CargarCommand.Execute(null);
    }
}