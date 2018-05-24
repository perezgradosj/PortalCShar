using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using PortalCShar.Clases;

namespace PortalCShar.Models
{
    public class UsuarioModel
    {
        Conexion con = new Conexion();
        SqlConnection conexion;
        SqlCommand comando;
        SqlDataReader reader;

        public string RegistraClienteEmpresa(Usuario x) {

            string mensaje = null;
            conexion = con.getConexion();
            try
            {
                conexion.Open();
                comando = new SqlCommand("[Mtro].[Usp_RegistrarClienteEmpresa]", conexion);
                comando.CommandType = CommandType.StoredProcedure;

                comando.Parameters.AddWithValue("@NroDocumento", x.Ndocumento);
                comando.Parameters.AddWithValue("@RazonSocial", x.razonSocial);
                comando.Parameters.AddWithValue("@Email", x.correo);
                comando.Parameters.AddWithValue("@Username", x.Ndocumento);
                comando.Parameters.AddWithValue("@Password", x.clave);
                comando.Parameters.AddWithValue("@IdTipoUsuario", x.tipoUser);
                comando.Parameters.AddWithValue("@IdEstado", x.estado);
                comando.Parameters.AddWithValue("@RucEmpresa", x.compañia);

                int resultado = comando.ExecuteNonQuery();
                mensaje = resultado == 0 ? "Error al Insertar" : "Registrado Correctamente";

                conexion.Close();
            }
            catch (Exception e) {

            }

            return mensaje;
        }

        public Boolean BuscarRucDni(string rucdni,string compania) {

            bool respuesta= false;
            conexion = con.getConexion();
            try
            {
                conexion.Open();
                comando = new SqlCommand("[Seg].[Usp_BuscarRucDni]", conexion);
                comando.CommandType = CommandType.StoredProcedure;

                comando.Parameters.AddWithValue("@rucdni", rucdni);
                comando.Parameters.AddWithValue("@compania", compania);

                reader = comando.ExecuteReader();

                if (reader.HasRows)
                    respuesta = true;

                conexion.Close();
            }
            catch (Exception e) {

            }

            return respuesta;
        }

        public Usuario Login(string usuario, string clave,string compañia) {

            Usuario x = null;
            conexion = con.getConexion();
            try
            {
                conexion.Open();
                comando = new SqlCommand("[Seg].[Usp_IniciarSesion]", conexion);
                comando.CommandType = CommandType.StoredProcedure;

                comando.Parameters.AddWithValue("@Username", usuario);
                comando.Parameters.AddWithValue("@Password", clave);
                comando.Parameters.AddWithValue("@Conpania", compañia);

                reader = comando.ExecuteReader();

                while (reader.Read()) {
                    x = new Usuario();
                    x.Ndocumento = reader.GetString(1);
                    x.razonSocial = reader.GetString(2);
                    x.correo = reader.GetString(3);
                    x.clave = reader.GetString(6);
                }

                conexion.Close();
            }
            catch (Exception e) {

            }

            return x;
        }

        public String ModificaDatos(string correonuevo, string clavenueva,string rucreceptor,string compania) {
            string mensaje = null;
            conexion = con.getConexion();
            try {
                conexion.Open();
                comando = new SqlCommand("[Seg].[Usp_ModificaDatos]", conexion);
                comando.CommandType = CommandType.StoredProcedure;

                comando.Parameters.AddWithValue("@correonuevo", correonuevo);
                comando.Parameters.AddWithValue("@clavenueva", clavenueva);
                comando.Parameters.AddWithValue("@rucreceptor", rucreceptor);
                comando.Parameters.AddWithValue("@compania", compania);

                int resultado = comando.ExecuteNonQuery();
                mensaje = resultado == 0 ? "Error al modificar" : "Datos modificados Correctamente";

                conexion.Close();
            }catch(Exception e)
            {

            }

            return mensaje;
        }
    }
}