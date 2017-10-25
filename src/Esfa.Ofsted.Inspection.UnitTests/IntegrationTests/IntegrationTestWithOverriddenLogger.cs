using System;
using Esfa.Ofsted.Inspection.Client;
using NUnit.Framework;

namespace Esfa.Ofsted.Inspection.UnitTests.IntegrationTests
{
    [TestFixture, Explicit]
    public class IntegrationTestWithOverriddenLogger
    {
        static bool? _processedWithLog;

        [Test]
        public void TestWithoutLogger()
        {

            _processedWithLog = false;
            var x = new OfstedInspectionsClient().GetOfstedInspectionOutcomes();
            Assert.IsFalse(_processedWithLog);
            Assert.AreEqual(0, x.InspectionOutcomeErrors.Count);
        }

        [Test]
        public void TestWithLogger()
        {

            var logger = new LocalLogger();
 
            _processedWithLog = false;
            var y = new OfstedInspectionsClient(logger).GetOfstedInspectionOutcomes();
            Assert.IsTrue(_processedWithLog);
            Assert.AreEqual(0, y.InspectionOutcomeErrors.Count);

        }

        private class LocalLogger : ILogFunctions
        {
            public Action<string> Debug { get; set; } = x =>
            {
                _processedWithLog = true;
                Console.WriteLine($@"Debugging: {x}");
            };

            public Action<string> Info { get; set; } = x => Console.WriteLine(x);
            public Action<string> Warn { get; set; } = x => Console.WriteLine(x);
            public Action<string, Exception> Error { get; set; } = (x, y) => Console.WriteLine($@"Error {x}, Exception Message: {y.Message}");
        }

    }
}
