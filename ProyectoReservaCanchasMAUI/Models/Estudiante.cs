using SQLite;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoReservaCanchasMAUI.Models
{
    public class Estudiante : PersonaUdla
    {
        public Estudiante()
        {
            TipoPersona = "Estudiante";
        }

        [Ignore] // Evita que SQLite la intente guardar
        public string NombreCarrera { get; set; } = string.Empty;
        public int CarreraId { get; set; }

        public bool Sincronizado { get; set; } = false;
    }
}
