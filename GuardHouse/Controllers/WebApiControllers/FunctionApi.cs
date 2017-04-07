using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

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

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            // Set the Method property of the request to POST.
            request.Method = "POST";
            // Create POST data and convert it to a byte array.
            string postData = string.Format("grant_type=password"
                                            + "&password={0}"
                                            + "&username={1}", password, userName);
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            // Set the ContentType property of the WebRequest.
            request.ContentType = "application/x-www-form-urlencoded";
            // Set the ContentLength property of the WebRequest.
            request.ContentLength = byteArray.Length;
            // Get the request stream.
            Stream dataStream = request.GetRequestStream();
            // Write the data to the request stream.
            dataStream.Write(byteArray, 0, byteArray.Length);
            // Close the Stream object.
            dataStream.Close();

            WebResponse response = request.GetResponse();
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);

                return reader.ReadToEnd();
            }
        }

        public static string GetCommunityInfo(string restapi)
        {
            try
            {
                string site = "";
                using (GuardHouse.Models.guardhouseEntities g = new GuardHouse.Models.guardhouseEntities())
                {
                    site = g.guardhouse.Select(s => s.condominiositio).FirstOrDefault();
                }

            
                if (HttpContext.Current.Session["bearer"] == null)
                {
                    HttpContext.Current.Session["bearer"] = GetToken(site);
                }

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
    }
}