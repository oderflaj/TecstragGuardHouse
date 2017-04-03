using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GuardHouse.Models;
using GuardHouse.Controllers;
using GuardHouse.Controllers.Shared;

namespace GuardHouse.Controllers
{
    [SessionExpirada] //Funcionalidad que verifica que el usuario se encuentre en sesión, si no lo esta abre pantalla login
    public class VisitasController : Controller
    {
        
        public ActionResult Visitas()
        {
            guardhouseEntities gh = new guardhouseEntities();
            Exception ex = new Exception();
            try
            {
                string fecActual = DateTime.Now.ToShortDateString();
                DateTime d = DateTime.Parse(fecActual);
                var visitax = gh.visita.Where(v => v.fecEntrada == d).OrderByDescending(o=>o.horaEntrada).ToList();
                ViewBag.visitas = visitax;
                ViewBag.registros = visitax.Count;

            }
            catch (Exception e)
            {
                ViewBag.Error = true;
                ViewBag.ErrorMensaje = e.Message;
                return View("Visitas");
            }
            //finally
            //{
            //    if (gh != null)
            //    {
            //        gh.Dispose();
            //    }
            //}
                return View();
        }

        public ActionResult Registro(string numeroid, string placa, string nombrevisitante, string tipoid, string marca, string submarca, string color, string casa, int idcasa, string direccion, string estado, string convenio, string mensaje)
        {
            guardhouseEntities gh = new guardhouseEntities();
            Exception ex = new Exception();
            try {
                if(numeroid.Trim().Length<4)
                {
                    ex = new Exception("El número de identificación debe ser de almenos 5 caracteres.");
                    throw ex;
                }

                if (placa.Trim().Length < 6)
                {
                    ex = new Exception("El número de placa debe ser de al menos 6 caracteres.");
                    throw ex;
                }


                if (nombrevisitante.Trim().Length < 10)
                {
                    ex = new Exception("El nombre del visitante debe ser de al menos 10 caracteres incluyendo apellidos registrados en la identificación.");
                    throw ex;
                }


                if (tipoid.Trim().Length < 3)
                {
                    ex = new Exception("No se recibio el tiopo de identificación.");
                    throw ex;
                }

                visitante invitado = new visitante();

                invitado = gh.visitante.Where(v => v.numeroid.Equals(numeroid.ToUpper()) && v.estatus.Equals(Label.activo)).FirstOrDefault();

                if (invitado == null)
                {
                    invitado = new visitante
                    {
                        numeroid = numeroid.ToUpper(),
                        tipoid = tipoid.ToUpper(),
                        nombre = nombrevisitante.ToUpper().Trim(),
                        estatus = Label.activo
                    };
                    gh.visitante.Add(invitado);
                    gh.SaveChanges();
                }

                string formaIng = "CAMINANDO";

                vehiculo coche = new vehiculo();

                if (!placa.Trim().ToUpper().Equals("CAMINANDO"))
                {
                    coche = gh.vehiculo.Where(c => c.placa.Equals(placa.ToUpper())).FirstOrDefault();
                    if(coche==null)
                    { 
                        coche = new vehiculo
                        {
                            placa = placa.ToUpper(),
                            marca = marca.ToUpper(),
                            submarca = submarca.ToUpper(),
                            color = color.ToUpper(),
                            idvisitante = invitado.id
                        };
                        gh.vehiculo.Add(coche);
                        gh.SaveChanges();
                    }
                    formaIng = string.Format("en automovil {0} {1} placa {2} color {3}", coche.marca, coche.submarca, coche.placa, coche.color);
                }

                puerta puerta = new puerta();
                turno turno = new turno();
                List<guardia> guardia = new List<guardia>();

                puerta = (Session["puerta"]!=null?(puerta)Session["puerta"] : null);
                turno = (Session["turno"]!=null?(turno)Session["turno"] : null);
                guardia = (Session["guardia"]!=null?(List<guardia>)Session["guardia"] : null);


                string detalle1 = string.Format("Visitante {0} con número de identificación {1} tipo {2} ingresa {3} por la puerta {4} dirección {5}. La entrada fue dada por el turno {6}.", invitado.nombre, invitado.numeroid, invitado.tipoid, formaIng, (puerta != null? puerta.no.ToString() :""), (puerta != null ? puerta.direccion : ""), (turno !=null ? turno.noTurno.ToString() : "")  );
                string detalle2 = "Información de guardias que permitieron el acceso: ";

                foreach (var g in guardia)
                {
                    detalle2 += string.Format("Nombre: {0}, Empresa:{1}, Puesto: {2} |", g.nombre, g.empresa, g.puesto);
                }


                visita visitax = new visita
                {
                    fecEntrada = DateTime.Now,
                    horaEntrada = DateTime.Now.ToString("HH:mm"),
                    idvisitante = invitado.id,
                    idpropiedad = idcasa,
                    estatuspropiedad = estado,
                    detalle1 = detalle1,
                    detalle2 = detalle2,
                    nopropiedad = casa,
                    domicilio = direccion + " #"+casa

                };

                if (!formaIng.Equals("CAMINANDO"))
                {
                    visitax.idvehiculo = coche.id;
                }

                gh.visita.Add(visitax);
                gh.SaveChanges();

            }
            catch(Exception e)
            {
                ViewBag.Error = true;
                ViewBag.ErrorMensaje = "Error al guardar el registro de la visita. " + e.Message;
                return RedirectToAction("Visitas", "Visitas");
            }
            finally {
                if (gh != null)
                {
                    gh.Dispose();
                }
            }

            //return View("Visitas", "Visitas");
            return RedirectToAction("Visitas", "Visitas");
        }
    }
}