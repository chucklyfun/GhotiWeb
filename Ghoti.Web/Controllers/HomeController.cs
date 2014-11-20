using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

namespace ghoti.web.Controllers
{
    public class HomeController : Nancy.NancyModule
    {
        public HomeController()
        {
            Get["/"] = _ => View["content/index"];
        }
    }
}