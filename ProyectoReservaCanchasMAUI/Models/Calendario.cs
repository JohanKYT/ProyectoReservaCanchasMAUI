using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoReservaCanchasMAUI.Models
{
    public class Calendario
    {
        [PrimaryKey, AutoIncrement]
        public int CalendarioId { get; set; }

        public DateTime FechaHoraInicio { get; set; }
        public DateTime FechaHoraFin { get; set; }

        public string Estado { get; set; } = "Pendiente";
        public string NotasDetallada { get; set; } = string.Empty;

        public int CanchaId { get; set; }
        public int PersonaUdlaId { get; set; }

        public bool Sincronizado { get; set; } = false;
    }
}
