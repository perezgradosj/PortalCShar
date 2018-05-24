using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data.SqlClient;

namespace PortalCShar.Models
{
    public class Conexion
    {
        public SqlConnection getConexion() {

            //SqlConnection conexion = new SqlConnection(ConfigurationManager.ConnectionStrings["conexion"].ConnectionString); /*conexión antigua*/

            //1. Recupero variables de conexión
            string Fuente = ConfigurationManager.AppSettings["Fuente"].ToString();
            string BaseDatos = ConfigurationManager.AppSettings["BaseDatos"].ToString();
            string Usuario = ConfigurationManager.AppSettings["Usuario"].ToString();
            string Clave = ConfigurationManager.AppSettings["Clave"].ToString();

            //2. Armo la cadena de conexión
            string cadena = "Server=" + Fuente + ";Database=" + BaseDatos + ";User=" + Usuario + ";pwd=" + Clave;

            //3. Realizo la conexión
            SqlConnection conexion = new SqlConnection(cadena);

            return conexion;
        }
    }
}