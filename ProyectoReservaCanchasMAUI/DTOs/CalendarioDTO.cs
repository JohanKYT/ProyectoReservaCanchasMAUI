using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoReservaCanchasMAUI.DTOs
{
    public class CalendarioDTO
    {
        public int CalendarioId { get; set; }
        public DateTime FechaHoraInicio { get; set; }
        public DateTime FechaHoraFin { get; set; }
        public string Estado { get; set; } = "Pendiente";
        public string NotasDetallada { get; set; } = string.Empty;

        public int CanchaId { get; set; }
        public string? NombreCancha { get; set; } // Para mostrar en picker

        public int PersonaUdlaId { get; set; }
        public string? NombrePersona { get; set; } // Para mostrar en picker
    }
}
