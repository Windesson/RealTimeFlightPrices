using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using Flightscraper.Spa.App_Start;
using Flightscraper.Spa.Conf;
using Flightscraper.Spa.Data;
using Unity;

namespace Flightscraper.Spa
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            // Create unity container 
            UnityConfig.RegisterContainer(new UnityContainer());

            GlobalConfiguration.Configuration.DependencyResolver = new UnityResolver(UnityConfig.Container);

            // Here your usual Web API configuration stuff.
            GlobalConfiguration.Configure(WebApiConfig.Register);

            //Load airports to memory
            AirportRepository.LoadAirports();
        }
    }
}
