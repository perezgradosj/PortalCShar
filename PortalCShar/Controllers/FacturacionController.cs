using System;
using System.Collections.Generic;
using System.Web.Mvc;
using PortalCShar.Models;
using PortalCShar.Clases;
using System.Configuration;
using System.Net.Mail;
using System.Net;
using System.Net.Mime;

namespace PortalCShar.Controllers
{
    public class FacturacionController : Controller
    {
        //Invoco DocumentoModel
        DocumentoModel DocumentoModel = new DocumentoModel();
        

        //Invoco webservice
        ServiceFacturacion.ServicioFacturacionClient ServicioFacturacion = new ServiceFacturacion.ServicioFacturacionClient();
        ServiceConsult.WServiceGetDocumentSoapClient ServiceConsult = new ServiceConsult.WServiceGetDocumentSoapClient();

        public void hola() { }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Index(string tipodocumento, string serie, string ndocumento, string fechaemision, string montototal,string tipo)
        {

            string rucemisor = ConfigurationManager.AppSettings["RucEmisor"].ToString();
            string fechasql = Convert.ToDateTime(fechaemision).ToString("dd-MM-yyyy");

            //1. Consultamos los documentos antiguos del portal.
            bool respuesta = DocumentoModel.ConsultaDocumento(rucemisor, tipodocumento, serie, ndocumento, fechasql, montototal);

            if (respuesta)
            {
                //2. Si encontro el documento, entonces lo traemos.
                return Json(respuesta);
            }
            else {
                //3. Sino, buscamos entre los nuevos documentos.
                string num_cpe = rucemisor + "-" + tipodocumento + "-" + serie + "-" + ndocumento;
                string xmlbase64 = ServicioFacturacion.GetXMLPortal(num_cpe, fechasql, montototal);                              

                if (xmlbase64.Length < 58)
                    return Json(false);
                else {
                    string ruta = ConfigurationManager.AppSettings["Ruta"].ToString();                    
                    if (tipo == "xml")
                    {
                        byte[] xmlbytes = Convert.FromBase64String(xmlbase64);
                        System.IO.File.WriteAllBytes(ruta + num_cpe + ".xml", xmlbytes);

                        return Json(true);
                    }
                    else
                    {
                        string pdfbase64 = ServiceConsult.GetDocumentoPDF(num_cpe);
                        byte[] pdfbytes = Convert.FromBase64String(pdfbase64);
                        System.IO.File.WriteAllBytes(ruta + num_cpe + ".pdf", pdfbytes);

                        return Json(true);
                    }
                }                
            }                
        }

        public ActionResult ConsultaDocumento()
        {
            return View();
        }

        [HttpPost]
        public JsonResult ConsultaDocumento(string fechainicio,string fechafin,string tipodocumento)
        {
            string fechainiciosql = Convert.ToDateTime(fechainicio).ToString("dd-MM-yyyy");
            string fechafinsql = Convert.ToDateTime(fechafin).ToString("dd-MM-yyyy");
            string compania = ConfigurationManager.AppSettings["RucEmisor"].ToString();
            Usuario x = Session["usuario"] as Usuario;
            string rucreceptor = x.Ndocumento;

            List<Documento> lista= DocumentoModel.ConsultaDocumentoFechas(fechainiciosql, fechafinsql, tipodocumento,compania, rucreceptor);
            var lista_nueva = ServicioFacturacion.ListaDocumentosPortal(rucreceptor, compania, fechainiciosql, fechafinsql, tipodocumento);

            if (lista.Count == 0) {
                //Si no hay documento antiguos, entonces busco entre los nuevos.                
                return Json(lista_nueva);
            }
            else
            {
                //Si encontrasemos documentos antiguos, aún asi debemos de buscar tambien entre los nuevos.
                if (lista_nueva.Length > 0) {
                    Documento documento = null;
                    for (int i = 0; i < lista_nueva.Length; i++)
                    {
                        documento = new Documento();
                        documento.num_cpe = lista_nueva[i].num_cpe.ToString();
                        documento.seriecorrelativo = lista_nueva[i].seriecorrelativo.ToString();
                        documento.des_tipodocumento = lista_nueva[i].des_tipodocumento.ToString();
                        documento.fechaemision = lista_nueva[i].fechaemision.ToString();
                        documento.montototal = lista_nueva[i].montototal;
                        lista.Add(documento);
                    }
                }                
                return Json(lista);
            } 
        }

        [HttpPost]
        public JsonResult DescargaDocumento(string num_cpe) {

            string[] data = num_cpe.Split('.');

            if (data[1] == "xml")
            {
                //1. Voy a buscar en la carpeta "Mis documentos".
                string ruta = ConfigurationManager.AppSettings["Ruta"].ToString();
                if (System.IO.File.Exists(ruta + num_cpe))
                {
                    //2. Si esta el documento que busco, entonces lo traigo.
                    return Json(true);
                }

                else
                {
                    //2. Si no esta el documento que busco, entonces lo creo.
                    string xmlbase64 = ServiceConsult.GetDocumentoXML(data[0]);
                    byte[] xmlbytes = Convert.FromBase64String(xmlbase64);
                    System.IO.File.WriteAllBytes(ruta + num_cpe, xmlbytes);
                    return Json(true);
                }
            }
            else {
                //1. Voy a buscar en la carpeta "Mis documentos".
                string ruta = ConfigurationManager.AppSettings["Ruta"].ToString();
                if (System.IO.File.Exists(ruta + num_cpe))
                {
                    //2. Si esta el documento que busco, entonces lo traigo.
                    return Json(true);
                }

                else
                {
                    //2. Si no esta el documento que busco, entonces lo creo.
                    string pdfbase64 = ServiceConsult.GetDocumentoPDF(data[0]);
                    byte[] pdfbytes = Convert.FromBase64String(pdfbase64);
                    System.IO.File.WriteAllBytes(ruta + num_cpe, pdfbytes);
                    return Json(true);
                }
            }

            
        }        

        [HttpPost]
        public JsonResult EnvioCorreo(string correo,List<Documento> array) {

            string empresa = ConfigurationManager.AppSettings["RucEmisor"].ToString();
            string correoemisor = ConfigurationManager.AppSettings["correoemisor"].ToString();
            string contraseña = ConfigurationManager.AppSettings["contraseña"].ToString();

            Usuario x = Session["usuario"] as Usuario;
            string correoenvio = x.correo;

            string[] data_correo = correo.Split(';');
            bool respuesta = false;

            using (SmtpClient cliente = new SmtpClient("smtp.gmail.com", 587))
            {
                cliente.EnableSsl = true;
                cliente.Credentials= new NetworkCredential(correoemisor, contraseña);

                MailMessage message = new MailMessage();

                message.From = new MailAddress(correoenvio, correoenvio);

                for (int i = 0; i < data_correo.Length; i++) {
                    message.To.Add(new MailAddress(data_correo[i]));
                }

                
                //message.CC.Add(new MailAddress("jonathanperezgrados@gmail.com"));

                message.Subject = "Facturas electrónicas";

                string url_logo;
                if (empresa == "20302241598")
                    url_logo = "http://kmmp.com.pe/images/themexpert/logo-black.png";
                else
                    url_logo = "http://www.cumminsperu.pe/images/themexpert/logo-black.png";

                message.Body = "<html>" +
                                "<head></head>" +
                                "<body>" +
                                    "<center><img src='"+url_logo+"'></center>" +
                                    "<center><h2>Le fueron enviados los siguientes documentos</h2></center>" +
                                    "<table align='center'>" +
                                        "<thead style='background:#94c3f7;' align='center'>" +
                                            "<tr>" +
                                                "<th style='padding: 10px;'>Documento</th>" +
                                                "<th style='padding: 10px;'>Tipo documento</th>" +
                                                "<th style='padding: 10px;'>Fecha emision</th>" +
                                                "<th style='padding: 10px;'>Monto total</th>" +
                                            "</tr>" +
                                        "<tbody align='center'>";
                    for (int i = 0; i < array.Count ; i++) {
                        message.Body += "<tr>";
                        message.Body += "<td style='padding: 10px;'>" + array[i].seriecorrelativo + "</td>";
                        message.Body += "<td style='padding: 10px;'>" + array[i].des_tipodocumento + "</td>";
                        message.Body += "<td style='padding: 10px;'>" + array[i].fechaemision + "</td>";
                        message.Body += "<td style='padding: 10px;'>" + array[i].monto + "</td>";
                        message.Body += "</tr>";
                    }
                message.Body += "</tbody></table></body></html>";
                
                message.IsBodyHtml = true;

                string ruta = ConfigurationManager.AppSettings["Ruta"].ToString();
                Attachment archivo_xml;
                Attachment archivo_pdf;
                for (int i = 0; i < array.Count; i++)
                {
                    ExistenArchivos(array[i].num_cpe);
                    archivo_xml = new Attachment(ruta + array[i].num_cpe + ".xml");
                    archivo_pdf = new Attachment(ruta + array[i].num_cpe + ".pdf");
                    message.Attachments.Add(archivo_xml);
                    message.Attachments.Add(archivo_pdf);
                }
                
                try
                {
                    cliente.Send(message);
                    respuesta = true;
                    return Json(respuesta);
                }
                catch (Exception e) {
                    return Json(e.Message + " " + e.InnerException);
                }
            }
                
        }

        public void ExistenArchivos(string num_cpe)
        {
            string ruta = ConfigurationManager.AppSettings["Ruta"].ToString();
            if (System.IO.File.Exists(ruta + num_cpe + ".xml"))
            {
                //Si existe, no hago nada.
            }

            else
            {
                //Si no existe, entonces lo creo.
                string xmlbase64 = ServiceConsult.GetDocumentoXML(num_cpe);
                byte[] xmlbytes = Convert.FromBase64String(xmlbase64);
                System.IO.File.WriteAllBytes(ruta + num_cpe + ".xml", xmlbytes);
            }
            if (System.IO.File.Exists(ruta + num_cpe + ".pdf"))
            {
                //Si existe, no hago nada.
            }

            else
            {
                //Si no existe, entonces lo creo.
                string pdfbase64 = ServiceConsult.GetDocumentoPDF(num_cpe);
                byte[] pdfbytes = Convert.FromBase64String(pdfbase64);
                System.IO.File.WriteAllBytes(ruta + num_cpe + ".pdf", pdfbytes);
            }
        }

    }
}