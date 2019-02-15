using System;
using System.Web;
using System.Web.Security;

namespace Guoc.BigMall.Admin.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        public void SignIn(string userName, string accountInfo, bool createPersistentCookie)
        {
            var now = DateTime.Now;

            var ticket = new FormsAuthenticationTicket(
                1 /*version*/,
                userName,
                now,
                now.Add(FormsAuthentication.Timeout),
                createPersistentCookie,
                accountInfo,
                FormsAuthentication.FormsCookiePath);

            var encryptedTicket = FormsAuthentication.Encrypt(ticket);

            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket)
            {
                HttpOnly = true,
                Secure = FormsAuthentication.RequireSSL,
                Path = FormsAuthentication.FormsCookiePath
            };

            if (FormsAuthentication.CookieDomain != null)
            {
                cookie.Domain = FormsAuthentication.CookieDomain;
            }

            if (createPersistentCookie)
            {
                cookie.Expires = ticket.Expiration;
            }

            HttpContext.Current.Response.Cookies.Add(cookie);
        }
        public void SignOut()
        {
            FormsAuthentication.SignOut();

            //var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, "")
            //{
            //    Expires = DateTime.Now.AddYears(-1),
            //};

            //HttpContext.Current.Response.Cookies.Add(cookie);
        }
    }
}