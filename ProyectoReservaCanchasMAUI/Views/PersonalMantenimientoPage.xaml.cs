
using ProyectoReservaCanchasMAUI.ViewModels;

namespace ProyectoReservaCanchasMAUI.Views;

public partial class PersonalMantenimientoPage : ContentPage
{
    private readonly PersonalMantenimientoViewModel _viewModel;
    public PersonalMantenimientoPage()
	{
        InitializeComponent();
        // Obtener el ViewModel usando inyección de dependencias
        _viewModel = App.Current?.Handler?.MauiContext?.Services.GetService<PersonalMantenimientoViewModel>()
                     ?? throw new Exception("AdministradorViewModel no pudo ser resuelto");
        BindingContext = _viewModel;
        // Ejecutar el comando de carga inicial
        _viewModel.CargarCommand.Execute(null);
    }
}