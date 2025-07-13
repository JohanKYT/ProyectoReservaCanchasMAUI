using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoReservaCanchasMAUI.Models
{
    public class Campus
    {
        [PrimaryKey, AutoIncrement]
        public int CampusId { get; set; }
        [MaxLength(100)]
        public string Nombre { get; set; }
        [MaxLength(100)]
        public string Direccion { get; set; }
        public bool Sincronizado { get; set; } = false; 
    }
}
