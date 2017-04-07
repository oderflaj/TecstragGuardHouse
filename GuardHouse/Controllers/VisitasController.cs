using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GuardHouse.Models;
using GuardHouse.Controllers;
using GuardHouse.Controllers.Shared;
using System.Text.RegularExpressions;

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

                if (Session["ErrorRegistro"] != null && (bool)Session["ErrorRegistro"] == true)
                {
                    ViewBag.Error = true;
                    ViewBag.ErrorMensaje = Session["ErrorRegistroMensaje"];
                }

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
            Session["ErrorRegistro"] = null;
            Session["ErrorRegistroMensaje"] = null;

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
                else if(invitado.tipoid.Equals(tipoid.ToUpper()) && !invitado.nombre.Equals(nombrevisitante.ToUpper().Trim()))
                {
                    ex = new Exception("Esta registrando un VISITANTE previamente registrado con el mismo número y tipo de IDENTIFICACION y diferente NOMBRE. Si el nombre del visitante es muy distinto al registrado apunte los datos de la IDENTIFICACION a mano y reporte al administrador y seleccione el valor existente para dejarlo pasar; si el nombre no cambia mucho solo seleccione el valor existente.");
                    Session["idVisitanteAlerta"] = numeroid.ToUpper();
                    Session["alertag"] = string.Format("Visitante SOSPECHOSO: ID->{0} Tipo->{1} Nombre->{2}.", numeroid.ToUpper(), tipoid.ToUpper(), nombrevisitante.ToUpper().Trim());
                    throw ex;
                }

                string formaIng = "CAMINANDO";

                vehiculo coche = new vehiculo();
                //Regex rgx = new Regex("[^a-zA-Z0-9 -]");
                Regex rgx = new Regex("[^a-zA-Z0-9]");
                placa = rgx.Replace(placa, "").ToUpper();

                if (!placa.Equals("CAMINANDO"))
                {
                    
                    coche = gh.vehiculo.Where(c => c.placa.Equals(placa)).FirstOrDefault();

                    if (coche == null)
                    {


                        coche = new vehiculo
                        {
                            placa = placa,
                            marca = marca.ToUpper(),
                            submarca = submarca.ToUpper(),
                            color = color.ToUpper(),
                            idvisitante = invitado.id
                        };
                        gh.vehiculo.Add(coche);
                        gh.SaveChanges();
                    }
                    else if (!coche.color.Equals(color.ToUpper().Trim()) || !coche.marca.Equals(marca.ToUpper().Trim()) || !coche.submarca.Equals(submarca.ToUpper().Trim()))
                    {
                        ex = new Exception("Esta registrando un Vehiculo previamente registrado con el mismo número de placa. Si el auto varia en SUBMODELO ó COLOR apunte los datos a mano y reporte al administrador y seleccione el valor existente; de lo contrario solo seleccione el valor existente.");
                        Session["placa"] = placa;
                        Session["alertav"] = string.Format("Automovil SOSPECHOSO: Placa->{0} Marca->{1} Submarca->{2} Color->{3}.",placa, marca.ToUpper(), submarca.ToUpper(), color.ToUpper());
                        throw ex;
                    }
                    else if (coche.idvisitante != invitado.id)
                    {
                        vehiculo newCoche = new vehiculo
                        {
                            placa = placa,
                            marca = marca.ToUpper(),
                            submarca = submarca.ToUpper(),
                            color = color.ToUpper(),
                            idvisitante = invitado.id
                        };
                        gh.vehiculo.Add(newCoche);
                        gh.SaveChanges();
                        coche = newCoche;
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

                //Se agrega información del visitante como sospechosa
                if (Session["idVisitanteAlerta"]!=null && Session["idVisitanteAlerta"].ToString().Equals(numeroid.ToUpper()))
                {
                    detalle2 += " | Alerta " + Session["alertag"].ToString();
                    Session["idVisitanteAlerta"] = "";
                    Session["alertag"] = "";
                }
                

                //Se agrega información del vehiculo como sospechosa
                if (Session["placa"]!=null && Session["placa"].ToString().Equals(placa))
                {
                    detalle2 += " | Alerta " + Session["alertav"].ToString();
                    Session["placa"] = "";
                    Session["alertav"] = "";
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
                Session["ErrorRegistro"] = true;
                Session["ErrorRegistroMensaje"] = "Error al guardar el registro de la visita. " + e.Message;
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