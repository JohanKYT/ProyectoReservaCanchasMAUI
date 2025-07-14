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
        [PrimaryKey]
        public int CanchaId { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public string Tipo { get; set; } = string.Empty;

        public bool Disponible { get; set; } = true;

        public int CampusId { get; set; } 

        public bool Sincronizado { get; set; } = false;  // Para sincronización
                                                        
        // Propiedad NO mapeada, solo para mostrar en UI
        [Ignore] // Para que SQLite la ignore
        public string NombreCampus { get; set; } = string.Empty;

    }
}
