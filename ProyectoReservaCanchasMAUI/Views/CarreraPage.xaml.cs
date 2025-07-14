
using ProyectoReservaCanchasMAUI.ViewModels;

namespace ProyectoReservaCanchasMAUI.Views;

public partial class CarreraPage : ContentPage
{
    private readonly CarreraViewModel _viewModel;

    public CarreraPage()
	{
		InitializeComponent();

        _viewModel = App.Current?.Handler?.MauiContext?.Services.GetService<CarreraViewModel>()
                     ?? throw new Exception("CanchaViewModel no pudo ser resuelto");

        BindingContext = _viewModel;

        _viewModel.CargarCommand.Execute(null);
    }
}