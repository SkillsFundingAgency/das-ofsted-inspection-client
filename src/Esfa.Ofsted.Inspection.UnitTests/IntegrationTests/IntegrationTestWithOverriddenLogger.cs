using System;
using Esfa.Ofsted.Inspection.Client.ApplicationServices;
using NUnit.Framework;

namespace Esfa.Ofsted.Inspection.UnitTests.IntegrationTests
{
    [TestFixture, Explicit]
    public class IntegrationTestWithOverriddenLogger
    {

        [Test]
        public void TestWithoutLogger()
        {

            var resultWithoutLogger = new GetOfstedInspections().GetAll();
        }

        [Test]
        public void TestWithLogger()
        {

            var logger = new LocalLogger();
            var resultWithLogger = new GetOfstedInspections(logger).GetAll();

            Assert.IsTrue(true);

        }

        private class LocalLogger : ILogFunctions
        {
            public Action<string> Debug { get; set; } = x =>
            {
                Console.WriteLine($@"Debugging: {x}");
            };

            public Action<string> Info { get; set; } = x => Console.WriteLine(x); 
            public Action<string, Exception> Error { get; set; } = (x, y) => Console.WriteLine($@"Error {x}, Exception Message: {y.Message}");
        }

    }
}
