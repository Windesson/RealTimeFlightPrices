using System.Collections.Generic;
using Flightscraper.Spa.ApiServices;
using Flightscraper.Spa.Data;
using Flightscraper.Spa.Interfaces;
using Flightscraper.Spa.Utilities;
using Unity;
using Unity.Injection;

namespace Flightscraper.Spa.Conf
{
    public static class UnityConfig
    {
        public static IUnityContainer Container { get; set; }

        public static void RegisterContainer(IUnityContainer container)
        {
            Container = container;

            Container.RegisterType<ILoggerWrapper, LoggerWrapper>(TypeLifetime.Singleton);
            Container.RegisterType<IQuoteIdGenerator, QuoteIdGenerator>(TypeLifetime.Singleton);
            Container.RegisterType<IAirportRepository, AirportRepository>(TypeLifetime.Singleton);
            Container.RegisterType<ISkyscannerLoader, SkyscannerLoader>(TypeLifetime.Singleton);
            Container.RegisterType<IPageLoader, PageLoader>(TypeLifetime.Singleton);
            Container.RegisterType<IFlightResponseReader, KayakResponseReader>("Kayak", new InjectionConstructor());
            Container.RegisterType<IFlightResponseReader, SkyscannerResponseReader>("Skyscanner", new InjectionConstructor());

            container.RegisterFactory<List<IFlightResponseReader>>(
                m => new List<IFlightResponseReader>
                {
                    m.Resolve<IFlightResponseReader>("Kayak"),
                    m.Resolve<IFlightResponseReader>("Skyscanner")
                }
            );

            Container.RegisterType<IFlightRepository, FlightRepository>(new InjectionConstructor(
                new ResolvedParameter<List<IFlightResponseReader>>()
             ));

        }
    }
}