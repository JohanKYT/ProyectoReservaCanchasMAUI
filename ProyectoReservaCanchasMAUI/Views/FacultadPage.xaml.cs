using ProyectoReservaCanchasMAUI.Services;
using ProyectoReservaCanchasMAUI.Data;
using ProyectoReservaCanchasMAUI.ViewModels;

namespace ProyectoReservaCanchasMAUI.Views;

public partial class FacultadPage : ContentPage
{
    public FacultadPage()
    {
        InitializeComponent();
        // Obtener el servicio ya creado por MAUI
        var facultadService = App.Current?.Handler?.MauiContext?.Services.GetService<FacultadService>();

        if (facultadService != null)
        {
            var viewModel = new FacultadViewModel(facultadService);
            BindingContext = viewModel;
            viewModel.CargarCommand.Execute(null); // opcional
        }
        else
        {
            // Manejo de error si no se pudo obtener el servicio
            Console.WriteLine("FacultadService no está disponible.");
        }
    }
    
}
