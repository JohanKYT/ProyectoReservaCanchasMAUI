using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoReservaCanchasMAUI.Models.NewFolder
{
    public class Facultad
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int CampusId { get; set; }
        public Campus? Campus { get; set; }
    }
}
