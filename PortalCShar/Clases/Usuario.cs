using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortalCShar.Clases
{
    public class Usuario
    {

        public string Ndocumento { get; set; }
        public string razonSocial { get; set; }
        public string correo { get; set; }
        public string contraseña { get; set; }
        public string usuario { get; set; }
        public string clave { get; set; }
        public int tipoUser { get; set; }
        public int estado { get; set; }
        public string compañia { get; set; }
    }
}