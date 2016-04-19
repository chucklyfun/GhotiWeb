using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Owin;
using Owin;
using Microsoft.AspNet.SignalR;

namespace Ghoti.Web
{
    [assembly: OwinStartup(typeof(Ghoti.Web.Startup))]
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var hubConfiguration = new HubConfiguration();
            hubConfiguration.EnableDetailedErrors = true;

            app.MapSignalR(hubConfiguration);
        }
    }
}