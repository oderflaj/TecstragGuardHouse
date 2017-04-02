using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using GuardHouse.Models;
using GuardHouse.Controllers.Shared;

namespace GuardHouse.Seguridad
{
    public class AuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated(); // 
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            using (guardhouseEntities cmt = new guardhouseEntities())
            {
                var usuario = cmt.usuario.Where(u => u.email.Equals(context.UserName)).FirstOrDefault();
                var condominio = cmt.guardhouse.FirstOrDefault();

                if (usuario == null)
                {
                    context.SetError("invalid_grant", "El usuario no existe en la base de datos");
                    return;
                }
                else if (!usuario.estatus.Equals("activo"))
                {
                    context.SetError("invalid_grant", "El usuario no esta activo");
                    return;
                }
                else if (!usuario.password.Equals(Function.GetPw(context.Password)))
                {
                    context.SetError("invalid_grant", "El password es incorrecto");
                    return;
                }
                else
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role, usuario.perfil.ToString()));
                    identity.AddClaim(new Claim("username", usuario.email));
                    //identity.AddClaim(new Claim(ClaimTypes.Name, usuario.nombre + " " + usuario.apepaterno));
                    identity.AddClaim(new Claim(ClaimTypes.Uri, condominio.condominiositio));
                    //identity.AddClaim(new Claim("apepaterno", usuario.apepaterno));
                    //identity.AddClaim(new Claim("apematerno", usuario.apematerno));
                    identity.AddClaim(new Claim(ClaimTypes.Email, usuario.email));
                    identity.AddClaim(new Claim("id", usuario.id.ToString()));
                    //identity.AddClaim(new Claim("telefono", usuario.id.ToString()));
                    context.Validated(identity);
                }
            }
        }
    }
}