using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GuardHouse.Models;
using GuardHouse.Controllers.Shared;

namespace GuardHouse.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            return View("LoginUser");
        }

        public ActionResult Sign(usuario usx, string tk)
        {
            guardhouseEntities gh = new guardhouseEntities();

            var password = Function.GetPw(usx.password.Trim());

            //Se inicializan las banderas de error
            ViewBag.Error = false;
            Exception ex = new Exception();

            ViewBag.email = usx.email.Trim().ToLower();

            try
            {
                //Validamos el email
                if (!Function.IsValidEmail(ViewBag.email))
                {
                    ex = new Exception("El email tiene un formato invalido.");
                    throw ex;
                }

                var users = gh.usuario.Where(u => u.email.Equals(usx.email.ToLower().Trim()) && u.estatus.Equals(Label.activo)).ToList();

                //Validación del usuario
                if (users.Count == 0)
                {
                    ex = new Exception("No se encontró usuario activo con el email proporcionado.");
                    throw ex;
                }

                if (users.Count > 1)
                {
                    ex = new Exception("Existe mas de un usuario activo con el mismo email.");
                    throw ex;
                }

                if (!users[0].password.Equals(password))
                {
                    ex = new Exception("El password ingresado es incorrecto.");
                    throw ex;
                }

                var usuario = users[0];

                //Se carga información base de la Caseta
                var caseta = gh.guardhouse.FirstOrDefault();

                if (caseta == null)
                {
                    ex = new Exception("No existe información de configuración de la Caseta.");
                    throw ex;
                }
                var responContacto = " Comuniquese con el encargado " + caseta.responsable + " al teléfono: " + caseta.telefonoresponsable + " para que complemente la información necesaria.";

                //Se carga perfil
                var perfil = gh.perfil.Where(p => p.valor == usuario.perfil && p.estatus.Equals(Label.activo)).FirstOrDefault();

                if (perfil == null)
                {
                    ex = new Exception("No existe un Perfil para el usuario." + responContacto);
                    throw ex;
                }

                //Sección para gente LOCAL de la CASETA
                List<turno> turno = new List<Models.turno>();
                puerta puerta = new Models.puerta();
                List<guardia> guardia = new List<Models.guardia>();

                if (usuario.perfil == Label.GUARDIA)
                {
                    turno = gh.turno.Where(t => t.estatus.Equals(Label.activo) && t.idUsuario == usuario.id).ToList();

                    if (turno.Count == 0)
                    {
                        ex = new Exception("No existe un Turno relacionado con el usuario. Comuniquese con el encargado." + responContacto);
                        throw ex;
                    }


                    if (turno.Count > 1)
                    {
                        ex = new Exception("Existe mas de un Turno activo relacionado con el usuario. Comuniquese con el encargado." + responContacto);
                        throw ex;
                    }

                    puerta = turno[0].puerta;//gh.puerta.Where(p => p.estatus.Equals(Label.activo) && p.id == turno[0].idPuerta).ToList();

                    if (puerta==null)
                    {
                        ex = new Exception("No existe una Puerta relacionada con el Turno y el Usuario. Comuniquese con el encargado." + responContacto);
                        throw ex;
                    }


                    if (puerta == null)
                    {
                        ex = new Exception("Existe mas de una Puerta relacionada a un Turno activo y el usuario." + responContacto);
                        //throw ex;
                    }

                    guardia = gh.guardia.Where(g => g.estatus.Equals(Label.activo) && g.idusuario == usuario.id).ToList();

                    if (guardia.Count == 0)
                    {
                        ex = new Exception("No existe Ninguna información personal de los guardias relacionada con el Usuario." + responContacto);
                        throw ex;
                    }


                }

                //Se cargan las variables de sesion 
                //Usuario
                Session["id"] = usuario.id;
                Session["email"] = usuario.email;
                Session["perfiluser"] = usuario.perfil;
                Session["usuario"] = usuario;

                //Caseta
                Session["idCaseta"] = caseta.id;
                Session["condominio"] = caseta.condominio;
                Session["condominiositio"] = caseta.condominiositio;
                Session["clave"] = caseta.clave;
                Session["direccion"] = caseta.direccion;
                Session["emailresponsable"] = caseta.emailresponsable;
                Session["responsable"] = caseta.responsable;
                Session["telefonoresponsable"] = caseta.telefonoresponsable;
                Session["telefonocaseta"] = caseta.telefono;
                Session["guardhouse"] = caseta;

                //Perfil
                Session["perfil"] = perfil;
                Session["nombrePerfil"] = perfil.nombre;

                //token
                Session["tk"] = tk;

                //Seccion de los guardias
                if (usuario.perfil == Label.GUARDIA)
                {
                    Session["turno"] = turno[0];
                    Session["puerta"] = puerta;
                    Session["guardia"] = guardia;
                    Session["noPuerta"] = puerta.no;
                    Session["dirPuerta"] = puerta.direccion;
                }

                //Se carga el menu
                List<modulo> menuList = new List<modulo>();
                modulo menu = new modulo();

                var modulos = gh.modulo.Where(m => m.estatus.Equals(Label.activo)).ToList();

                foreach (var m in modulos)
                {
                    if (m.perfiles.Contains(usuario.perfil + ","))
                    {
                        menuList.Add(m);
                    }
                }

                //Se almacena el login en la bitacora
                using (Bitacora bit = new Bitacora())
                {
                    bit.Loginout(Bitacora.LOG_.Login);
                }
                    
                

                Session["menu"] = menuList;

                //Se redirecciona a la pagina que se tiene por default
                return RedirectToAction(perfil.defaultmodulo.ToString(), "Modulos");



            }
            catch (Exception e)
            {
                ViewBag.Error = true;
                ViewBag.ErrorMensaje = e.Message;
                return View("LoginUser");

            }
            finally
            {
                if (gh != null)
                    gh.Dispose();
            }

        }

        public ActionResult Logout(usuario um)
        {
            Session.Abandon();
            //Se almacena el login en la bitacora
            using (Bitacora bit = new Bitacora())
            {
                bit.Loginout(Bitacora.LOG_.Logout);
            }
            ViewBag.Info = true;
            ViewBag.InfoMensaje = "Se ha cerrado la sesión exitosamente";
            return View("LoginUser");
        }


        public ActionResult Timeout()
        {
            Session.Abandon();
            //Se almacena el login en la bitacora
            using (Bitacora bit = new Bitacora())
            {
                bit.Loginout(Bitacora.LOG_.Logout);
            }
            ViewBag.Info = true;
            ViewBag.InfoMensaje = "Se termino el tiempo de sesión!";
            return View("LoginUser");
        }
    }
}