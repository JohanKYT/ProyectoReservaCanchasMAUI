using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoReservaCanchasMAUI.Models
{
    public class Facultad
    {
        [PrimaryKey, AutoIncrement]
        public int FacultadId { get; set; }
        public string Nombre { get; set; }
        public int CampusId { get; set; }
        [Ignore] // <-- evita que SQLite la intente guardar
        public string NombreCampus { get; set; } = string.Empty;
        public bool Sincronizado { get; set; } = false;
    }
}
