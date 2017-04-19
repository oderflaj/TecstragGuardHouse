using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using GuardHouse.Models;
using GuardHouse.Controllers;
using GuardHouse.Controllers.Shared;

namespace GuardHouse.Controllers.WebApiControllers
{
    [RoutePrefix("api/data")]
    public class HistoricoVisitasApiController : ApiController
    {
        [Authorize(Roles = "1")]
        [HttpGet]
        [Route("HistoricoVisitas/{from}/{to}")]
        public IHttpActionResult GetHistoricoVisitas(string from, string to)
        {
            guardhouseEntities gh = new guardhouseEntities();
            try
            {
                DateTime fromx = DateTime.Parse(from);
                DateTime tox = DateTime.Parse(to);

                var visitax = gh.visita.Where(v=>v.fecEntrada>= fromx && tox >= v.fecEntrada).ToList();

                var visitas = visitax.Select(s=> new {
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

                return Content(HttpStatusCode.OK, new { visitas });
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.BadRequest, e.Message);
            }
            finally
            {
                if (gh != null)
                {
                    gh.Dispose();
                }
            }
        }
    }
}
