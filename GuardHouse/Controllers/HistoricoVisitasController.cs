using GuardHouse.Controllers.Shared;
using GuardHouse.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace GuardHouse.Controllers
{
    [SessionExpirada] //Funcionalidad que verifica que el usuario se encuentre en sesión, si no lo esta abre pantalla login
    public class HistoricoVisitasController : Controller
    {
        // GET: HistoricoVisitas
        public ActionResult HistoricoVisitas(string from = null, string to=null, int print=0)
        {

            DateTime ahora = DateTime.Now;

            DateTime desde = (from == null ? ahora.AddMonths(-1) : DateTime.Parse(from));
            DateTime hasta = (to == null ? ahora.AddMonths(1) : DateTime.Parse(to));

            if (print == 1)
            {
                guardhouseEntities gh = new guardhouseEntities();
                try
                {
                    var visitax = gh.visita.Where(v => v.fecEntrada >= desde && hasta >= v.fecEntrada).ToList();

                    var visitas = visitax.Select(s => new
                    {
                        Fecha_Entrada = s.fecEntrada.ToString("yyyy-MM-dd"),
                        Hora_Entrada = s.horaEntrada,
                        Domicilio = s.domicilio,
                        Estatus = s.estatuspropiedad,
                        Nombre = s.nombre,
                        Id = s.numeroid,
                        TipoId = s.tipoid,
                        Placa = s.placa,
                        Marca = s.marca,
                        Submarca = s.submarca,
                        Color = s.color,
                        Usuario_Caseta = s.usuarioguardhouse,
                        Detalle = s.detalle1,
                        Subdetalle = s.detalle2
                    }).ToList();


                    ViewBag.rango = desde.ToString("dd/MM/yyyy") + " - " + hasta.ToString("dd/MM/yyyy");
                    ViewBag.reporte = new JavaScriptSerializer().Serialize(visitas);// visitas;// new JavaScriptSerializer().Serialize(visitas);
                    return View("HistoricoVisitasPrint");

                }
                catch (Exception e)
                {
                    ViewBag.Error = true;
                    ViewBag.ErrorMensaje = "Error al generar la impresión del reporte.";
                    return View();
                }
                finally
                {
                    if (gh != null)
                    {
                        gh.Dispose();
                    }
                }
            }
            else
            {
                ViewBag.rango = desde.ToString("dd/MM/yyyy") + " - " + hasta.ToString("dd/MM/yyyy");
                ViewBag.from = desde.ToString("dd/MM/yyyy");
                ViewBag.to = hasta.ToString("dd/MM/yyyy");
                ViewBag.fromapi = desde.ToString("yyyy-MM-dd");
                ViewBag.toapi = hasta.ToString("yyyy-MM-dd");
                ViewBag.excel = string.Format("Visitas_{0}_", Session["condominio"]);
            }
            return View();
        }
    }
}