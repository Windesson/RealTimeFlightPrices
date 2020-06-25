using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using Castle.Core.Internal;
using Flightscraper.Spa.Conf;
using Flightscraper.Spa.Interfaces;
using Flightscraper.Spa.Models;
using log4net;
using Unity;

namespace Flightscraper.Spa.Data
{
    /// <summary>
    /// Custom database initializer class used to populate
    /// the database with seed data.
    /// </summary>
    internal class DatabaseInitializer : CreateDatabaseIfNotExists<Context>
    {
        private static readonly ILog Logger;

        static DatabaseInitializer()
        {
            Logger = UnityConfig.Container.Resolve<ILoggerWrapper>().GetLogger(typeof(DatabaseInitializer));
        }

        protected override void Seed(Context context)
        {
            var filePath = System.Web.HttpContext.Current.Server.MapPath("~/OpenFlights/airports.dat");

            try
            {
                using (var reader = new StreamReader(filePath))
                {
                    string line = "";
                    reader.ReadLine();
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] values = line.Split(',');

                        var iataCode = values[4].Replace("\"", "");
                        var city = values[2].Replace("\"", "");
                        var country = values[3].Replace("\"", "");

                        //we only want airport that contain the below information.
                        if (iataCode.Length != 3 || city.IsNullOrEmpty() || country.IsNullOrEmpty()) continue;

                        var airport = new Airport();
                        if (int.TryParse(values[0], out var id))
                        {
                            airport.Id = id;
                        }

                        airport.City = city;
                        airport.Country = country;
                        airport.IATA = iataCode;

                        context.Airports.Add(airport);
                    }
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"{ex.Message}|{ex.InnerException}");
            }
        }

    }
}