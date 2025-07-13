using ProyectoReservaCanchasMAUI.ViewModels;

namespace ProyectoReservaCanchasMAUI.Views;

public partial class SincronizacionPage : ContentPage
{
    private readonly SincronizacionViewModel _viewModel;

    public SincronizacionPage(SincronizacionViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    private async void OnSincronizarClicked(object sender, EventArgs e)
    {
        await _viewModel.SincronizarTodoAsync();
    }
}