using GuardHouse.Controllers.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GuardHouse.Controllers
{
    [SessionExpirada]
    public class ModulosController : Controller
    {
        // GET: Modulos
        public ActionResult AdmiDashboard()
        {
            return RedirectToAction("AdmiDashboard", "AdmiDashboard");
        }


        public ActionResult Visitas()
        {
            return RedirectToAction("Visitas", "Visitas");
        }


        public ActionResult Agenda()
        {
            return RedirectToAction("Agenda", "Agenda");
        }

    }
}