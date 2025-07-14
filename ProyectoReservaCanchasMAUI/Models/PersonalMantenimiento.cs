using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoReservaCanchasMAUI.Models
{
    public class PersonalMantenimiento : PersonaUdla
    {
        public PersonalMantenimiento()
        {
            TipoPersona = "Personal de Mantenimiento";

        }
        public bool Sincronizado { get; set; } = false;
    } 
}
