using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Mvc;
using System.Web.Routing;
using WebApp.DAL;
using WebApp.DAL.Data;

namespace WebApp.App_Start.Attributes
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        public List<string> Roles { get; set; }
        ApplicationUserManager UserManager = new ApplicationUserManager(new Microsoft.AspNet.Identity.EntityFramework.UserStore<ApplicationUser>(ApplicationDbContext.Create()));
        public CustomAuthorizeAttribute(string roles)
        {
            Roles = new List<string>();
            Roles.AddRange(roles.Split(',').ToList());
        }
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var isAuthorized = base.AuthorizeCore(httpContext);
            if (isAuthorized)
                if (Roles.Any(a => HttpContext.Current.User.IsInRole(a)))
                {
                    return true;
                }
            return false;
        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (!HttpContext.Current.User.Identity.IsAuthenticated)
            {
                base.HandleUnauthorizedRequest(filterContext);
            }
            else
            {
                if (HttpContext.Current.User.IsInRole("Viewer"))
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                    {
                        controller = "Viewer",
                        action = "Index"
                    }));
                else
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                    {
                        controller = "Home",
                        action = "Index"
                    }));
            }
        }
    }
}