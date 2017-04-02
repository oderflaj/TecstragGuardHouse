using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GuardHouse.Models;

namespace GuardHouse.Controllers.Shared
{
    [SessionExpirada] //Funcionalidad que verifica que el usuario se encuentre en sesión, si no lo esta abre pantalla login
    public class Bitacora : IDisposable
    {

        private usuario Usuario { get; set; }
        private perfil Perfil { get; set; }
        private guardhouse Condominio { get; set; }
        private turno Turno { get; set; }
        private puerta Puerta { get; set; }
        private List<guardia> Guardias { get; set; }
        public enum LOG_ {
            Login,
            Logout
        };

        /// <summary>
        /// Crea una instancia de la bitacora utilizando las variables de sesion
        /// </summary>
        public Bitacora()
        {
            try
            {
                this.Usuario = (usuario)HttpContext.Current.Session["usuario"];
                this.Perfil = (perfil)HttpContext.Current.Session["perfil"];
                this.Condominio = (guardhouse)HttpContext.Current.Session["guardhouse"];

                if (this.Usuario.perfil == Label.GUARDIA)
                {
                    this.Turno = (turno)HttpContext.Current.Session["turno"];
                    this.Puerta = (puerta)HttpContext.Current.Session["puerta"];
                    this.Guardias = (List<guardia>)HttpContext.Current.Session["guardia"];
                }
            }
            catch(Exception e)
            {
                e = new Exception("Error al intentar cargar los datos de sesión, intente de nuevo.");
                //throw e;
            }
            
        }
        /// <summary>
        /// Crea una instancia de la bitacora con parametro inicial del usuario, esta sobrecarga se debe usar cuando se va a modificar a un usuario distinto al que esta en sesión
        /// </summary>
        /// <param name="idusuario">ID del usuario al que se le esta modificando la informacion</param>
        public Bitacora(int idusuario)
        {
            guardhouseEntities gh = new guardhouseEntities();
            try
            {
                this.Usuario = gh.usuario.Where(u => u.id == idusuario).FirstOrDefault();
                this.Perfil = gh.perfil.Where(p => p.valor == this.Usuario.perfil && p.estatus.Equals(Label.activo)).FirstOrDefault();
                this.Condominio = (guardhouse)HttpContext.Current.Session["guardhouse"];
                if (this.Usuario.perfil == Label.GUARDIA)
                {
                    this.Turno = gh.turno.Where(t => t.idUsuario == idusuario && t.estatus.Equals(Label.activo)).FirstOrDefault();
                    this.Puerta = gh.puerta.Where(p=>p.id==this.Turno.idPuerta && p.estatus.Equals(Label.activo)).FirstOrDefault();
                    this.Guardias = gh.guardia.Where(g=>g.idusuario == idusuario && g.estatus.Equals(Label.activo)).ToList();
                }


            }
            catch (Exception e)
            {
                e = new Exception("Error al obtener el perfil del usuario.");
                throw e;
            }
            finally
            {
                if (gh != null)
                {
                    gh.Dispose();
                }
            }
        }

        

        public void Loginout(LOG_ act)
        {
            guardhouseEntities gh = new guardhouseEntities();
            try
            {
                var bit = new usuariobitacora
                {
                    idusuario = this.Usuario.id,
                    email = this.Usuario.email,
                    accion = ( act ==LOG_.Login ? "LOGIN" : "LOGOUT"),
                    detalle = (act == LOG_.Login ? "Inicio de sesión en el sistema" : "Cierre de sesión en el sistema"),
                    fecha = DateTime.Now,
                    hora = DateTime.Now.ToString("HH:mm"),
                    infoguardia = ""
                };

                if (this.Usuario.perfil == Label.GUARDIA)
                {
                    foreach(var guard in this.Guardias)
                    {
                        bit.infoguardia += string.Format("Nombre: {0}| Puesto: {1}| Empresa: {2}| ", guard.nombre,guard.puesto, guard.empresa);
                    }

                    bit.infoguardia += string.Format("Turno:{0}| Puerta:{1}| Dirección:{2}", this.Turno.noTurno,this.Turno.puerta.no, this.Turno.puerta.direccion);

                }
                gh.usuariobitacora.Add(bit);
                gh.SaveChanges();

            }
            catch (Exception e)
            {
                e = new Exception("Error al obtener el perfil del usuario.");
                throw e;
            }
            finally
            {
                if (gh != null)
                {
                    gh.Dispose();
                }
            }
        }

        public void Dispose()
        {
            //base.Dispose();
        }


    }
}