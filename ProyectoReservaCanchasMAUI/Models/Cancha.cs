using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoReservaCanchasMAUI.Models
{
    public class Cancha
    {
        [PrimaryKey, AutoIncrement]
        public int CanchaId { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public string Tipo { get; set; } = string.Empty;

        public bool Disponible { get; set; } = true;
        public bool Sincronizado { get; set; } = false;  
    }
}
