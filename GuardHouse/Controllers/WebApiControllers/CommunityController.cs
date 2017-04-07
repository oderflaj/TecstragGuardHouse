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
    public class CommunityController : ApiController
    {
        [AllowAnonymous]
        [HttpGet]
        [Route("forall")]
        public IHttpActionResult Get()
        {
            return Ok("Now server time is: " + DateTime.Now.ToString());
        }
        [Authorize(Roles = "1,3,5")]
        [HttpGet]
        [Route("estatus")]
        public IHttpActionResult GetEstatusNormal()
        {
            var identity = (ClaimsIdentity)User.Identity;
            return Ok("Hola Normal Usuario " + identity.Name);
        }
        [HttpGet]
        [Route("PropiedadEstatus/{numero}")]
        public IHttpActionResult GetPropiedadEstatus(string numero)
        {
            //string site = "";
            //using (GuardHouse.Models.guardhouseEntities g = new guardhouseEntities())
            //{
            //    site = g.guardhouse.Select(s => s.condominiositio).FirstOrDefault();
            //}

            //string url = string.Format("http{0}://{1}", (System.Configuration.ConfigurationManager.AppSettings.Get("SSH").ToString().Equals("si") ? "s" : ""), site);
            //url += "/api/data/PropiedadEstatusBackend/" + numero.ToString();
            //string respuesta = "";
            //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            //try
            //{
            //    WebResponse response = request.GetResponse();
            //    using (Stream responseStream = response.GetResponseStream())
            //    {
            //        StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);

            //        respuesta = reader.ReadToEnd();
            //    }
            //}
            //catch (WebException ex)
            //{
            //    WebResponse errorResponse = ex.Response;
            //    using (Stream responseStream = errorResponse.GetResponseStream())
            //    {
            //        StreamReader reader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
            //        String errorText = reader.ReadToEnd();
            //        // log errorText
            //    }
            //    return Content(HttpStatusCode.BadRequest, ex.Message);
            //}
            string respuesta = "";
            try
            {
                string restapi = "/api/data/PropiedadEstatusBackend/" + numero.ToString();
                respuesta = FunctionApi.GetCommunityInfo(restapi);
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, ex.Message);
            }

            
            return Ok(respuesta);
        }

        
    }
}
