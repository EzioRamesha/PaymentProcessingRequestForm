using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApp.DAL.Data;

namespace WebApp.App_Start.Attributes
{
    public class PermissionAttribute : ActionFilterAttribute
    {
        private List<string> searchPermissions;

        public PermissionAttribute(params string[] permissions)
        {
            searchPermissions = new List<string>();
            searchPermissions.AddRange(permissions.ToList());
        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            bool hasPermission = false;
            var permissions = HttpContext.Current.User.Identity.GetPermissions();
            if (permissions != null)
            {
                if (searchPermissions.Any(a => permissions.Contains(a)))
                {
                    hasPermission = true;                    
                }
            }
            if (hasPermission)
            {
                base.OnActionExecuting(filterContext);
            }
            else
            {
                filterContext.Result = new JsonResult
                {
                    Data = new
                    {
                        ResponseType = "error",
                        Message = "User does not have permission(s) to perform the action"
                    },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
        }
    }
}