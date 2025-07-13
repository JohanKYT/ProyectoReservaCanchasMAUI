using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoReservaCanchasMAUI.Interfaces
{
    public interface ISincronizable
    {
        Task SincronizarLocalesConApiAsync();
        Task SincronizarDesdeApiAsync();
    }
}
