using Esfa.Ofsted.Inspection.Client.ApplicationServices;
using Esfa.Ofsted.Inspection.Client.Services;
using NUnit.Framework;

namespace Sfa.Das.Ofsted.Inspection.UnitTests
{
    [TestFixture]
    public class InpectionsXlsxServiceTests
    {
        [Test]
        public void ShouldReturnResultsWithoutErroring()
        {
            var getOfstedInspections = new GetOfstedInspections(new ProcessExcelFormulaToLink());

            var res = getOfstedInspections.GetAll();


            Assert.IsTrue(true);

        }
    }
}
