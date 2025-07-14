using ProyectoReservaCanchasMAUI.ViewModels;

namespace ProyectoReservaCanchasMAUI.Views;

public partial class CalendarioPage : ContentPage
{
    private readonly CalendarioViewModel _viewModel;

    public CalendarioPage()
	{
		InitializeComponent();
        // Obtener el ViewModel usando inyección de dependencias
        _viewModel = App.Current?.Handler?.MauiContext?.Services.GetService<CalendarioViewModel>()
                     ?? throw new Exception("CalendarioViewModel no pudo ser resuelto");

        BindingContext = _viewModel;

        _viewModel.CargarCommand.Execute(null);
    }
}