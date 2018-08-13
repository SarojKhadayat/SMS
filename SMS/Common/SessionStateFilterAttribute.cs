using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SMS.Common
{
    public class SessionStateFilterAttribute:ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // check if session is supported
             var userInfo = filterContext.HttpContext.Session["email"];
           // var userInfo = filterContext.HttpContext.Request.Cookies["userInfo"];
            if (userInfo == null)
            {
                // check if a new session id was generated
                filterContext.Result = new RedirectResult("~/Account/Login");
                return;
            }
            base.OnActionExecuting(filterContext);
        }
    }
}