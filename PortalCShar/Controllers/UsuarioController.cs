using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PortalCShar.Clases;
using PortalCShar.Models;
using System.Configuration;

namespace PortalCShar.Controllers
{
    public class UsuarioController : Controller
    {
        UsuarioModel model = new UsuarioModel();
        Encrypt Encrypt = new Encrypt();

        // GET: Login
        public ActionResult Login()
        {
            return View();
        }

        public ActionResult CerrarSesion()
        {
            Session.Abandon();
            return RedirectToAction("Login");
        }

        [HttpPost]
        public JsonResult Login(string usuario,string clave)
        {
            string compania= ConfigurationManager.AppSettings["RucEmisor"].ToString();

            Usuario x = model.Login(usuario, Encrypt.EncryptKey(clave), compania);

            if (x == null)
                return Json(false);
            else {
                Session["usuario"] = new Usuario();
                Session["usuario"] = x;
                return Json(x);
            }

        }

        [HttpPost]
        public JsonResult Registro(string nombre, string ruc, string correo, string clave)
        {
            string compania= ConfigurationManager.AppSettings["RucEmisor"].ToString();
            bool verifica_dniruc = model.BuscarRucDni(ruc,compania);

            if (verifica_dniruc)
                return Json(verifica_dniruc);
            else {
                Usuario x = new Usuario();

                x.razonSocial = nombre;
                x.Ndocumento = ruc;
                x.usuario = ruc;
                x.correo = correo;
                x.contraseña = "";
                x.clave = Encrypt.EncryptKey(clave); 
                x.tipoUser = 1;
                x.estado = 1;
                x.compañia = compania;

                string respuesta = model.RegistraClienteEmpresa(x);

                return Json(respuesta);
            }           
        }

        [HttpPost]
        public JsonResult ModificaDatos(string correonuevo, string clavenueva)
        {
            string compania = ConfigurationManager.AppSettings["RucEmisor"].ToString();

            Usuario x = Session["usuario"] as Usuario;
            string rucreceptor = x.Ndocumento;
            string correoactual = x.correo;
            string claveactual = x.clave;

            string clavenueva_encriptada= Encrypt.EncryptKey(clavenueva);

            string resultado = null;
            //1. Si solo el correonuevo esta lleno
            if(correonuevo!=""&& clavenueva=="")
                resultado = model.ModificaDatos(correonuevo, claveactual, rucreceptor, compania);

            //2. Si solo la clavenueva esta llena
            else if (correonuevo == "" && clavenueva != "")
                resultado = model.ModificaDatos(correoactual, clavenueva_encriptada, rucreceptor, compania);

            //3. Si los dos estan llenos
            else
                resultado = model.ModificaDatos(correonuevo, clavenueva_encriptada, rucreceptor, compania);

            return Json(resultado);
        }
    }
}