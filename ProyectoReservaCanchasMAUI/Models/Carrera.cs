using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoReservaCanchasMAUI.Models
{
    public class Carrera
    {
        [PrimaryKey]
        public int CarreraId { get; set; }

        public string Nombre { get; set; }

        public int FacultadId { get; set; }

        [Ignore]
        public string NombreFacultad { get; set; }

        public bool Sincronizado { get; set; } = false;
    }
}
