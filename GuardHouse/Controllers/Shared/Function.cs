using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace GuardHouse.Controllers.Shared
{
    /// <summary>
    /// Clase que guarda las etiquetas a nivel global para estandarizar los valores
    /// </summary>
    public class Label
    {
        public const string activo = "activo";
        public const string pendiente = "pendiente";
        public const string cancelado = "cancelado";

        //Perfiles
        public const int ADMINISTRADOR = 1;
        public const int GUARDIA = 3;

        public Label()
        {

        }
    }

    public class Function
    {

        public Function()
        {

        }
        /// <summary>
        /// Convierte el pasword a una frase encriptada que es la que se encuentra almacenada en la base de datos utilizando la keyphrase
        /// </summary>
        /// <param name="texto">La cadena de caracteres que sera convertida a nuevo valor</param>
        /// <returns></returns>
        public static string GetPw(string texto)
        {
    
            return EncodeBytes(texto);

        }

        /// <summary>
        /// Desencripta el PW encriptado y almacenado para el usuario
        /// </summary>
        /// <param name="encriptado">Texto que representa el PW encriptado</param>
        /// <returns></returns>
        public static string DecPW(string encriptado)
        {
            return DecodeBytes(encriptado);
        }

        // Encode the specified byte array by using CryptoAPITranform.
        private static string EncodeBytes(string texto)
        {

            string key = System.Configuration.ConfigurationManager.AppSettings.Get("frase").ToString();

            //arreglo de bytes donde guardaremos la llave
            byte[] keyArray;

            //arreglo de bytes donde guardaremos el texto que vamos a encriptar
            byte[] Arreglo_a_Cifrar = UTF8Encoding.UTF8.GetBytes(texto);

            //se utilizan las clases de encriptación provistas por el Framework Algoritmo MD5
            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();

            //se guarda la llave para que se le realice hashing
            keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));

            hashmd5.Clear();

            //Algoritmo 3DAS
            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();

            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            //se empieza con la transformación de la cadena
            ICryptoTransform cTransform = tdes.CreateEncryptor();

            //arreglo de bytes donde se guarda la cadena cifrada
            byte[] ArrayResultado = cTransform.TransformFinalBlock(Arreglo_a_Cifrar, 0, Arreglo_a_Cifrar.Length);
            tdes.Clear();

            //se regresa el resultado en forma de una cadena
            return Convert.ToBase64String(ArrayResultado, 0, ArrayResultado.Length);
        }

        // Decode the specified byte array using CryptoAPITranform.
        private static string DecodeBytes(string textoEncriptado)
        {
            string key = System.Configuration.ConfigurationManager.AppSettings.Get("frase").ToString();
            byte[] keyArray;
            //convierte el texto en una secuencia de bytes
            byte[] Array_a_Descifrar = Convert.FromBase64String(textoEncriptado);

            //se llama a las clases que tienen los algoritmos
            //de encriptación se le aplica hashing
            //algoritmo MD5
            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();

            keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));

            hashmd5.Clear();

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();

            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateDecryptor();

            byte[] resultArray = cTransform.TransformFinalBlock(Array_a_Descifrar, 0, Array_a_Descifrar.Length);

            tdes.Clear();
            //se regresa en forma de cadena
            return UTF8Encoding.UTF8.GetString(resultArray);
        }

        public static string MakePw()
        {
            string bases = "ABC123";
            string seed = DateTime.Today.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
            Random rd = new Random(Int32.Parse(seed));

            bases += rd.Next().ToString();

            return bases;

        }

        /// <summary>
        /// Valida que el email contenga el formato correcto
        /// </summary>
        /// <param name="email">email a validar</param>
        /// <returns>True si es valido, False si es invalido</returns>
        public static bool IsValidEmail(string email)
        {
            try
            {
                var mail = new System.Net.Mail.MailAddress(email);
                return true;
            }
            catch
            {
                return false;
            }
        }

    }

    public class SessionExpirada : ActionFilterAttribute
    {
        //private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public override void OnActionExecuting(ActionExecutingContext filtercontext)
        {
            HttpContext ctx = HttpContext.Current;

            // check  sessions here
            if (HttpContext.Current.Session["id"] == null)
            {
                //filtercontext.Result = new RedirectResult("~/User/Logout");
                filtercontext.Result = new RedirectResult("~/Login/Timeout");
                //log.Info(string.Format("==FIN== Se cerro sesión TIMEOUT  para usuario:{0}", HttpContext.Current.Session["email"]));
                return;
            }
            base.OnActionExecuting(filtercontext);
        }

    }

}