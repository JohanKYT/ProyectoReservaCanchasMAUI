using ProyectoReservaCanchasMAUI.ViewModels;

namespace ProyectoReservaCanchasMAUI.Views;

public partial class CanchaPage : ContentPage
{
    private readonly CanchaViewModel _viewModel;

    public CanchaPage()
    {
        InitializeComponent();

        _viewModel = App.Current?.Handler?.MauiContext?.Services.GetService<CanchaViewModel>()
                     ?? throw new Exception("CanchaViewModel no pudo ser resuelto");

        BindingContext = _viewModel;

        _viewModel.CargarCommand.Execute(null);
    }
}