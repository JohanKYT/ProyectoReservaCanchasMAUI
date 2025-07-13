using ProyectoReservaCanchasMAUI.Interfaces;
using System.Collections.ObjectModel;

namespace ProyectoReservaCanchasMAUI.ViewModels
{
    public class SincronizacionViewModel : BaseViewModel
    {
        private readonly IEnumerable<ISincronizable> _servicios;

        private bool _isSincronizando;
        public bool IsSincronizando
        {
            get => _isSincronizando;
            set
            {
                _isSincronizando = value;
                OnPropertyChanged();
            }
        }

        public SincronizacionViewModel(IEnumerable<ISincronizable> servicios)
        {
            _servicios = servicios;
        }

        public async Task SincronizarTodoAsync()
        {
            IsSincronizando = true;

            foreach (var servicio in _servicios)
            {
                await servicio.SincronizarLocalesConApiAsync();
                await servicio.SincronizarDesdeApiAsync();
            }

            IsSincronizando = false;
        }
    }
}

