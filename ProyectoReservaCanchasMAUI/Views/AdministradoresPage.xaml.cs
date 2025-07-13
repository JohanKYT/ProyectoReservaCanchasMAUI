
using ProyectoReservaCanchasMAUI.ViewModels;

namespace ProyectoReservaCanchasMAUI.Views;

public partial class AdministradoresPage : ContentPage
{
    public AdministradoresPage()
    {
        InitializeComponent();

        var service = App.Current?.Handler?.MauiContext?.Services.GetService<AdministradorService>();

        if (service != null)
        {
            var viewModel = new AdministradorViewModel(service);
            BindingContext = viewModel;
            viewModel.CargarCommand.Execute(null);
        }
        else
        {
            Console.WriteLine("AdministradorService no está disponible.");
        }
    }
}


