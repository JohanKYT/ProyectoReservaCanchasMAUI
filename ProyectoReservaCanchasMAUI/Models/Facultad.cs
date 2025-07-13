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
        public Campus? Campus { get; set; }
        public bool Sincronizado { get; set; } = false;
    }
}
