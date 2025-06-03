using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoReservaCanchasMAUI.Models
{
    public class Administrador : PersonaUdla
    {
        public Administrador()
        {
            TipoPersona = "Administrador";
        }

        public int FacultadId { get; set; }
        public Facultad? Facultad { get; set; }
    }
}
