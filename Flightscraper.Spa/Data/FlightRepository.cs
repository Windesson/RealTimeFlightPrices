using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Castle.Core.Internal;
using Flightscraper.Spa.Conf;
using Flightscraper.Spa.Interfaces;
using log4net;
using Unity;

namespace Flightscraper.Spa.Data
{
    /// <summary>
    ///  A repository to fetch flights quotes from multiple services in real-time.
    /// </summary>
    public class FlightRepository : IFlightRepository
    {
        private readonly IList<IFlightResponseReader> _flightQuoteServiceCollaborators;
        private static readonly ILog Logger;

        /// <summary>
        /// Initialize unique logger using dependency injection
        /// </summary>
        static FlightRepository()
        {
            Logger = UnityConfig.Container.Resolve<ILoggerWrapper>().GetLogger(typeof(FlightRepository));
        }

        /// <summary>
        ///  Initialize master repository with real-time flight services  
        /// </summary>
        /// <param name="flightQuoteServiceCollaborators">list of Web services to provide real-time flight quotes</param>
        public FlightRepository(IList<IFlightResponseReader> flightQuoteServiceCollaborators)
        {
            if (flightQuoteServiceCollaborators.IsNullOrEmpty()) throw new InvalidOperationException("quoteServices cannot be null or empty");
            _flightQuoteServiceCollaborators = flightQuoteServiceCollaborators;
        }

        /// <summary>
        ///  A function to retrieve flight's quotes from multiple web services concurrently.
        ///  The search response is then joined from each individual flight services once the all search is completed.
        /// </summary>
        /// <param name="modelQuoteRequest">A model encapsulating a flight request</param>
        /// <returns></returns>
        public IEnumerable<IFlightResponse> BrowseQuotes(IFlightRequest modelQuoteRequest)
        {
            var responses = new List<IFlightResponse>();  //response place holder
            var childTasks = new List<Task<List<IFlightResponse>>>(); //list of task running concurrently 
            try
            {
                LogRequest(modelQuoteRequest);
                //parent task to hold response until all child task running are completed.
                var parentTask = StartSearch(modelQuoteRequest, childTasks);

                //wait for all child tasks to complete 
                parentTask.Wait();

                //join search response form each collaborator
                childTasks.ForEach(_ => responses.AddRange(_.Result));
            }
            catch (Exception ex)
            {
                LogErrors(ex, childTasks);
            }

            Logger.Info($"All search completed: responses found ({responses.Count})");
            return responses;
        }

        #region private methods

        private Task StartSearch(IFlightRequest modelQuoteRequest, ICollection<Task<List<IFlightResponse>>> childTasks)
        {
            return Task.Factory.StartNew(() =>
            {
                var childFactory = new TaskFactory(TaskCreationOptions.AttachedToParent, TaskContinuationOptions.None);
                foreach (var service in _flightQuoteServiceCollaborators)
                {
                    var serviceName = service.GetType().Name;
                    Logger.Info($"{serviceName} initiate service quote search.");
                    childFactory.StartNew(() =>
                    {
                        //search for flights 
                        var aTask = Task.Run(async () => await service.BrowseQuotesAsync(modelQuoteRequest));

                        //on search complete log search result count
                        aTask.GetAwaiter().OnCompleted(() =>
                            Logger.Info($"{serviceName} search completed. responses ({aTask.Result.Count})"));

                        //track child task
                        childTasks.Add(aTask);
                    });
                }
            });
        }

        private static void LogErrors(Exception ex, IEnumerable<Task<List<IFlightResponse>>> allSearchTask)
        {
            Logger.Error($"{ex.Message} | {ex.InnerException}");
            foreach (var task in allSearchTask)
            {
                if (task?.Exception?.InnerExceptions != null)
                    Logger.Error($"Flight quote child task error {task.Exception.Message} | {task.Exception.InnerExceptions}");
            }
        }

        private static void LogRequest(IFlightRequest modelQuoteRequest)
        {
            Logger.Info($"Initiating flight search from {modelQuoteRequest.Origin} to " +
                        $"{modelQuoteRequest.Destination} depart on {modelQuoteRequest.DepartDate} " +
                        $"return {modelQuoteRequest.ReturnDate}");
        }

        #endregion

    }
}
