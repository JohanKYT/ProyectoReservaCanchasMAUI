
using ProyectoReservaCanchasMAUI.ViewModels;

namespace ProyectoReservaCanchasMAUI.Views;

public partial class EstudiantePage : ContentPage
{
    private readonly EstudianteViewModel _viewModel;
    public EstudiantePage()
	{
		InitializeComponent();
        // Obtener el ViewModel usando inyección de dependencias
        _viewModel = App.Current?.Handler?.MauiContext?.Services.GetService<EstudianteViewModel>()
                     ?? throw new Exception("AdministradorViewModel no pudo ser resuelto");
        BindingContext = _viewModel;
        // Ejecutar el comando de carga inicial
        _viewModel.CargarCommand.Execute(null);
    }
}