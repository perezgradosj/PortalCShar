using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using PortalCShar.Clases;

namespace PortalCShar.Models
{
    public class DocumentoModel
    {
        Conexion con = new Conexion();
        SqlConnection conexion;
        SqlCommand comando;
        SqlDataReader reader;

        public Boolean ConsultaDocumento(string rucemisor, string tipodocumento, string serie, string ndocumento, string fechaemision, string montototal) {

            Boolean existe = false;
            conexion = con.getConexion();

            try
            {
                conexion.Open();
                comando = new SqlCommand("Fact.Usp_ExistenciaDocumento", conexion);
                comando.CommandType = CommandType.StoredProcedure;

                comando.Parameters.AddWithValue("@rucemisor", rucemisor);
                comando.Parameters.AddWithValue("@tipodocumento", tipodocumento);
                comando.Parameters.AddWithValue("@serie", serie);
                comando.Parameters.AddWithValue("@ndocumento", ndocumento);
                comando.Parameters.AddWithValue("@fechaemision", fechaemision);
                comando.Parameters.AddWithValue("@montototal", montototal);

                reader = comando.ExecuteReader();

                if (reader.HasRows)
                    existe = true;

            }
            catch (Exception e) {

            }

            return existe;
        }

        public List<Documento> ConsultaDocumentoFechas(string fechainicio, string fechafin, string tipodocumento,string compañia,string rucreceptor) {

            List<Documento> lista = new List<Documento>();
            Documento x = null;
            conexion = con.getConexion();
            try
            {
                conexion.Open();
                comando = new SqlCommand("[Fact].[Usp_ConsultaDocumentoFechas]", conexion);
                comando.CommandType = CommandType.StoredProcedure;

                comando.Parameters.AddWithValue("@fechainicio", fechainicio);
                comando.Parameters.AddWithValue("@fechafin", fechafin);
                comando.Parameters.AddWithValue("@tipodocumento", tipodocumento);
                comando.Parameters.AddWithValue("@compania", compañia);
                comando.Parameters.AddWithValue("@rucreceptor", rucreceptor);

                reader = comando.ExecuteReader();

                while (reader.Read()) {
                    x = new Documento();
                    x.num_cpe = reader.GetString(1);
                    x.seriecorrelativo = reader.GetString(2);
                    x.des_tipodocumento = reader.GetString(5);
                    x.fechaemision = (reader.GetDateTime(8)).ToString();
                    x.montototal =reader.GetDecimal(9);

                    lista.Add(x);
                }
            }
            catch (Exception e) {

            }
            return lista;
        }
    }
}