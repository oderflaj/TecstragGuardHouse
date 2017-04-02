using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GuardHouse.Models;
using GuardHouse.Controllers;
using GuardHouse.Controllers.Shared;

namespace GuardHouse.Controllers
{
    [SessionExpirada] //Funcionalidad que verifica que el usuario se encuentre en sesión, si no lo esta abre pantalla login
    public class VisitasController : Controller
    {
        
        public ActionResult Visitas()
        {
            return View();
        }

        public ActionResult Registro(string numeroid, string placa, string nombrevisitante, string tipoid, string marca, string submarca, string color)
        {
            guardhouseEntities gh = new guardhouseEntities();
            Exception ex = new Exception();
            try {
                if(numeroid.Trim().Length<4)
                {
                    ex = new Exception("El número de identificación debe ser de almenos 5 caracteres.");
                    throw ex;
                }

                if (placa.Trim().Length < 6)
                {
                    ex = new Exception("El número de placa debe ser de al menos 6 caracteres.");
                    throw ex;
                }


                if (nombrevisitante.Trim().Length < 10)
                {
                    ex = new Exception("El nombre del visitante debe ser de al menos 10 caracteres incluyendo apellidos registrados en la identificación.");
                    throw ex;
                }


                if (tipoid.Trim().Length < 3)
                {
                    ex = new Exception("No se recibio el tiopo de identificación.");
                    throw ex;
                }



            }
            catch(Exception e) {
            }
            finally {
                if (gh != null)
                {
                    gh.Dispose();
                }
            }

            return View("Visitas");
        }
    }
}