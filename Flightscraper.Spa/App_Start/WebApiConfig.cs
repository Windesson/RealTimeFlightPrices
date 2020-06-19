using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Flightscraper.Spa.Conf;
using Unity;

namespace Flightscraper.Spa
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "Airports",
                routeTemplate: "api/{controller}/{query}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "OneWayTrip",
                routeTemplate: "api/{controller}/{originPlace}/{destinationPlace}/{outboundDate}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "TwoWayTrip",
                routeTemplate: "api/{controller}/{originPlace}/{destinationPlace}/{outboundDate}/{inboundDate}",
                defaults: new { id = RouteParameter.Optional }
            );

            UnityConfig.RegisterContainer(new UnityContainer());
        }
    }
}
