using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Web.Http;

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
            //var identity = (ClaimsIdentity)User.Identity;
            //var sesion = identity.Claims
            //           .Where(c => c.Type == ClaimTypes.Role)
            //           .Select(c => c.Value)
            //           .ToList();
            string url = "http://"+System.Configuration.ConfigurationManager.AppSettings.Get("condominiositio").ToString();
            url += "/api/data/PropiedadEstatusBackend/" + numero.ToString();
            string respuesta = "";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            try
            {
                WebResponse response = request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    
                    respuesta = reader.ReadToEnd();
                }
            }
            catch (WebException ex)
            {
                WebResponse errorResponse = ex.Response;
                using (Stream responseStream = errorResponse.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
                    String errorText = reader.ReadToEnd();
                    // log errorText
                }
                throw ex;
            }
            
            //return Content(HttpStatusCode.BadRequest, "Error oook");
            return Ok(respuesta);
        }
    }
}
