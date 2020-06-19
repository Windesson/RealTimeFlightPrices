using Flightscraper.Spa.Interfaces;

namespace Flightscraper.Spa.Utilities
{
    public class QuoteIdGenerator : IQuoteIdGenerator
    {
        private int _id;

        public int GetId()
        {
            if (_id == int.MaxValue) _id = 0;
            else _id++;

            return _id;
        }
    }
}