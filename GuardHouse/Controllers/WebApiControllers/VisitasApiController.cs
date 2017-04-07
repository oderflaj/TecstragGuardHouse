using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Web.Http;
using GuardHouse.Models;
using GuardHouse.Controllers;
using GuardHouse.Controllers.Shared;

namespace GuardHouse.Controllers.WebApiControllers
{
    [RoutePrefix("api/data")]
    public class VisitasApiController : ApiController
    {
        [HttpGet]
        [Route("HistoricoVisitante/{numeroid}/{placa}")]
        public IHttpActionResult GetHistoricoVisitante(string numeroid, string placa)
        {

            guardhouseEntities gh = new guardhouseEntities();
            try
            {
                List<visitante> visi = new List<visitante>();
                List<vehiculo> vehi = new List<vehiculo>();
                List<Object> visitantes = new List<Object>();

                //if(placa.Trim().Length>3)
                //{
                vehi = gh.vehiculo.Where(v => v.placa.Contains(placa.Trim().ToUpper())).ToList();

                var x = (from v in vehi
                         select new
                         {
                             placa = v.placa,
                             marca = v.marca,
                             submarca = v.submarca,
                             color = v.color,
                             numeroid = v.visitante.numeroid,
                             tipoid = v.visitante.tipoid,
                             nombre = v.visitante.nombre,
                             id = v.visitante.id
                         }
                    );
                foreach (var i in x)
                {
                    visitantes.Add(i);
                }

                //}

                if (numeroid.Trim().Length > 3)
                {
                    var excludeIds = new HashSet<int>(x.Select(s => s.id)).ToList();
                    visi = gh.visitante.Where(v => v.numeroid.Contains(numeroid.ToUpper()) && v.estatus.Equals(Label.activo) && !excludeIds.Contains(v.id)).ToList();
                    var excludeVeh = new HashSet<int>((from v in visi
                                                       join vh in gh.vehiculo on v.id equals vh.idvisitante
                                                       select v.id)).ToList();
                    var xx = (from vx in visi
                              join vh in gh.vehiculo on vx.id equals vh.idvisitante
                              select new
                              {
                                  placa = vh.placa,
                                  marca = vh.marca,
                                  submarca = vh.submarca,
                                  color = vh.color,
                                  numeroid = vx.numeroid,
                                  tipoid = vx.tipoid,
                                  nombre = vx.nombre,
                                  id = vx.id
                              }
                              ).OrderBy(v=>v.placa).ToList();

                    //Se pasa abajo la integración
                    //foreach (var i in xx)
                    //{
                    //    visitantes.Add(i);
                    //}

                    var zz = (from vx in visi
                              where !excludeVeh.Contains(vx.id)
                              select new
                              {
                                  placa = "",
                                  marca = "",
                                  submarca = "",
                                  color = "",
                                  numeroid = vx.numeroid,
                                  tipoid = vx.tipoid,
                                  nombre = vx.nombre,
                                  id = vx.id
                              }
                              ).OrderBy(a=>a.numeroid).ToList();

                    foreach (var i in zz)
                    {
                        visitantes.Add(i);
                    }

                    //Se pone en segundo orden las placas
                    foreach (var i in xx)
                    {
                        visitantes.Add(i);
                    }
                }

                

                return Content(HttpStatusCode.OK, new { visitantes });


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
