using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoReservaCanchasMAUI.DTOs
{
    public class FacultadDTO
    {
        public int FacultadId { get; set; }
        public string Nombre { get; set; }
        public int CampusId { get; set; }
        public CampusDTO? Campus { get; set; }
    }
}
