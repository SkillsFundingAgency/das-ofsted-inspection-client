using Esfa.Ofsted.Inspection.Client.ApplicationServices;
using NUnit.Framework;

namespace Sfa.Das.Ofsted.Inspection.UnitTests
{
    [TestFixture]
    public class InpectionsXlsxServiceTests
    {
        [Test]
        public void ShouldReturnResultsWithoutErroring()
        {
            var getOfstedInspections = new GetOfstedInspections();

            var res = getOfstedInspections.GetAll();


            Assert.IsTrue(true);

        }
    }
}
