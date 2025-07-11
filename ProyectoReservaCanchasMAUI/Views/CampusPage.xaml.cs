using ProyectoReservaCanchasMAUI.Data;
using ProyectoReservaCanchasMAUI.Services;
using ProyectoReservaCanchasMAUI.ViewModels;

namespace ProyectoReservaCanchasMAUI.Views;

public partial class CampusPage : ContentPage
{
    public CampusPage()
    {
        InitializeComponent();

        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "campus.db");
        var database = new AppDatabase(dbPath);
        var service = new CampusService(database);
        var viewModel = new CampusViewModel(service);

        BindingContext = viewModel;
        viewModel.CargarCommand.Execute(null); // Opcional: carga al iniciar
    }
}