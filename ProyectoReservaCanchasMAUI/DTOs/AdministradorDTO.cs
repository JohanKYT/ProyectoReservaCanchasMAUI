using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoReservaCanchasMAUI.DTOs
{
    public class AdministradorDTO
    {
        public int BannerId { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Password { get; set; }
        public string Telefono { get; set; }
        public string Direccion { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string TipoPersona => "Administrador";

        public int FacultadId { get; set; }
        public FacultadDTO? Facultad { get; set; }
    }
}
