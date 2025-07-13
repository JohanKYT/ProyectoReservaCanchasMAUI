using ProyectoReservaCanchasMAUI.Data;
using ProyectoReservaCanchasMAUI.Services;
using ProyectoReservaCanchasMAUI.ViewModels;

namespace ProyectoReservaCanchasMAUI.Views;

public partial class CampusPage : ContentPage
{
    public CampusPage()
    {
        InitializeComponent();

        // Obtener el servicio ya creado por MAUI
        var campusService = App.Current?.Handler?.MauiContext?.Services.GetService<CampusService>();

        if (campusService != null)
        {
            var viewModel = new CampusViewModel(campusService);
            BindingContext = viewModel;
            viewModel.CargarCommand.Execute(null); // opcional
        }
        else
        {
            // Manejo de error si no se pudo obtener el servicio
            Console.WriteLine("CampusService no está disponible.");
        }
    }
}