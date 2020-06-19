using Flightscraper.Spa.Interfaces;
using Flightscraper.Spa.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlightScraper.UnitTests.Utilities
{
    [TestClass]
    public class QuoteIdGeneratorTests
    {
        [TestMethod]
        [TestCategory("Unit")]
        public void QuoteIdGenerator_WhenCalled_ReturnNewId()
        {
            //arrange
            IQuoteIdGenerator idGenerator = new QuoteIdGenerator();

            //act
            var expectedIdA = idGenerator.GetId();
            var expectedIdB = idGenerator.GetId();
            
            //Assert
            Assert.IsTrue(expectedIdA >= 0);
            Assert.IsTrue(expectedIdB > 0);
            Assert.AreNotEqual(expectedIdA, expectedIdB);
        }

        [TestMethod]
        [TestCategory("Unit"), TestCategory("ExpensiveTest")]
        public void QuoteIdGenerator_WhenMaxValueReached_ResetIdCount()
        {
            //arrange
            IQuoteIdGenerator idGenerator = new QuoteIdGenerator();

            //act
            var maxIdNumber = idGenerator.GetId();
            while (maxIdNumber != int.MaxValue)
            {
                maxIdNumber = idGenerator.GetId();
            }

            var expectedIdAfterMaxIdValue = idGenerator.GetId();

            //Assert
            Assert.IsTrue(expectedIdAfterMaxIdValue == 0);
        }
    }
}
