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

        _viewModel = App.Current?.Handler?.MauiContext?.Services.GetService<FacultadViewModel>()
                     ?? throw new Exception("FacultadViewModel no pudo ser resuelto");

        BindingContext = _viewModel;

        _viewModel.CargarCommand.Execute(null);
    }

}
