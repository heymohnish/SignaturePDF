﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SignaturePDF
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
             //defaults: new { controller = "Home", action = "Contact", id = UrlParameter.Optional }
             defaults: new { controller = "Login", action = "Index", id = UrlParameter.Optional }
              //defaults: new { controller = "Login", action = "DisplayPdf1", id = UrlParameter.Optional }
          //defaults: new { controller = "Login", action = "Details", id = UrlParameter.Optional } 
             );
        }
    }
}
