using System;
using System.Collections.Generic;
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
        public int CarreraId { get; set; }

        public bool Sincronizado { get; set; } = false;
    }
}
