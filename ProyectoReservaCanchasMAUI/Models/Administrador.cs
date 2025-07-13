using SQLite;
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
        [Ignore]
        public string NombreFacultad { get; set; } = string.Empty;
        public bool Sincronizado { get; set; } = false; 
    }
}
