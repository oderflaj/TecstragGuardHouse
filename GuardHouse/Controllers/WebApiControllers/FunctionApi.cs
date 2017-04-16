using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

namespace GuardHouse.Controllers.WebApiControllers
{
    public class FunctionApi
    {
        FunctionApi()
        {
        }

        private static string GetToken(string site)
        {
            string userName = System.Configuration.ConfigurationManager.AppSettings.Get("UserGuardHouse").ToString();
            string password = System.Configuration.ConfigurationManager.AppSettings.Get("PwGuardHouse").ToString();
            string url = string.Format("http{0}://{1}/token", (System.Configuration.ConfigurationManager.AppSettings.Get("SSH").ToString().Equals("si") ? "s" : ""), site);


            ///////
            url += string.Format("?grant_type=password&password={0}&username={1}", password, userName);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "text";
            // Clear out all buffers for the current writer 
            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Flush();
                streamWriter.Close();
            }

            // Get response  
            HttpWebResponse myWebResponse = (HttpWebResponse)request.GetResponse();
            // Get the response stream  
            StreamReader reader = new StreamReader(myWebResponse.GetResponseStream());

            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            var jsonObject = serializer.DeserializeObject(reader.ReadToEnd());

            string token = jsonObject.ToString();
            return token;
            ///////





            // Set the Method property of the request to POST.

            // Create POST data and convert it to a byte array.
            string postData = string.Format("grant_type=password"
                                            + "&password={0}"
                                            + "&username={1}", password, userName);
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            // Set the ContentType property of the WebRequest.
            
            // Set the ContentLength property of the WebRequest.
            request.ContentLength = byteArray.Length;
            // Get the request stream.
            Stream dataStream = request.GetRequestStream();
            // Write the data to the request stream.
            dataStream.Write(byteArray, 0, byteArray.Length);
            // Close the Stream object.
            dataStream.Close();

            
        }

        /// <summary>
        /// Funcion generica para consumir datos de sistema Community relacionado con Guardhouse
        /// </summary>
        /// <param name="restapi">URL complementaria al sitio destino, por ejemplo /api/data/PropiedadEstatusBackend/ + parametro</param>
        /// <returns>Resultado de la consulta Web Api, objeto JSON</returns>
        public static string GetCommunityInfo(string restapi)
        {
            try
            {
                string site = "";// HttpContext.Current.Session["condominiositio"].ToString().Trim();
                using (GuardHouse.Models.guardhouseEntities g = new GuardHouse.Models.guardhouseEntities())
                {
                    site = g.guardhouse.Select(s => s.condominiositio).FirstOrDefault();
                }

                //try
                //{
                //    if (HttpContext.Current.Session["bearer"] == null)
                //    {
                //        HttpContext.Current.Session["bearer"] = GetToken(site);
                //    }
                //}
                //catch (Exception e)
                //{
                //    HttpContext.Current.Session["bearer"] = GetToken(site);
                //}


                string url = string.Format("http{0}://{1}{2}", (System.Configuration.ConfigurationManager.AppSettings.Get("SSH").ToString().Equals("si") ? "s" : ""), site,restapi);
            
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            
                
                WebResponse response = request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);

                    return reader.ReadToEnd();
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
                //return Content(HttpStatusCode.BadRequest, ex.Message);
                throw ex;
            }
        }

        public static string PostCommunityInfo(string restapi, string objeto)
        {
            try
            {
                string site = "";// HttpContext.Current.Session["condominiositio"].ToString().Trim();
                using (GuardHouse.Models.guardhouseEntities g = new GuardHouse.Models.guardhouseEntities())
                {
                    site = g.guardhouse.Select(s => s.condominiositio).FirstOrDefault();
                }
                string url = string.Format("http{0}://{1}{2}", (System.Configuration.ConfigurationManager.AppSettings.Get("SSH").ToString().Equals("si") ? "s" : ""), site, restapi);

                using (var client = new WebClient())
                {
                    var values = new NameValueCollection();
                    values["object"] = objeto.ToString();//new JavaScriptSerializer().Serialize(objeto);
                   

                    client.Headers.Set("objeto", objeto);
                    
                    var response = client.UploadValues(url,"POST", values);

                    var responseString = Encoding.Default.GetString(response);

                    return responseString.ToString();
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
                //return Content(HttpStatusCode.BadRequest, ex.Message);
                throw ex;
            }
        }

    }
}