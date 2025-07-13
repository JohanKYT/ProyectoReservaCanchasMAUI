using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoReservaCanchasMAUI.DTOs
{
    public class CanchaDTO
    {
        public int CanchaId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public bool Disponible { get; set; }

        public int CampusId { get; set; }
        public CampusDTO? Campus { get; set; }
    }
}
