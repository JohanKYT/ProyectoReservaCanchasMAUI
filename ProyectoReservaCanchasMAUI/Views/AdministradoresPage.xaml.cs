
using ProyectoReservaCanchasMAUI.Services;
using ProyectoReservaCanchasMAUI.ViewModels;

namespace ProyectoReservaCanchasMAUI.Views;

public partial class AdministradoresPage : ContentPage
{
    public AdministradoresPage()
    {
        InitializeComponent();

        var administradorService = App.Current?.Handler?.MauiContext?.Services.GetService<AdministradorService>();
        var facultadService = App.Current?.Handler?.MauiContext?.Services.GetService<FacultadService>();

        if (administradorService != null && facultadService != null)
        {
            var viewModel = new AdministradorViewModel(administradorService, facultadService);
            BindingContext = viewModel;
            viewModel.CargarCommand.Execute(null);
        }
        else
        {
            Console.WriteLine("AdministradorService no está disponible.");
        }
    }
}


