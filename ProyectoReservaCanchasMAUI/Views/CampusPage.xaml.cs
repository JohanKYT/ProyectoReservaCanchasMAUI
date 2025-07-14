using ProyectoReservaCanchasMAUI.Data;
using ProyectoReservaCanchasMAUI.Services;
using ProyectoReservaCanchasMAUI.ViewModels;

namespace ProyectoReservaCanchasMAUI.Views;

public partial class CampusPage : ContentPage
{
    private readonly CampusViewModel _viewModel;

    public CampusPage()
    {
        InitializeComponent();

        _viewModel = App.Current?.Handler?.MauiContext?.Services.GetService<CampusViewModel>()
                     ?? throw new Exception("CampusViewModel no pudo ser resuelto");

        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (_viewModel != null)
            await _viewModel.CargarAsync();
    }
}
