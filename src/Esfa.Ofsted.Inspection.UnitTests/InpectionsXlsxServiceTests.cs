using Esfa.Ofsted.Inspection.Client.ApplicationServices;
using Esfa.Ofsted.Inspection.Client.Services;
using Moq;
using NUnit.Framework;
using Sfa.Das.Ofsted.Inspection.Types;

namespace Sfa.Das.Ofsted.Inspection.UnitTests
{
    [TestFixture]
    public class InpectionsXlsxServiceTests
    {
        [Test]
        public void ShouldReturnResultsWithoutErroring()
        {
            var mockProcessExcelFormulaToLink = new Mock<IProcessExcelFormulaToLink>();
            var mockOverallEffectivenessProcessor = new Mock<IOverallEffectivenessProcessor>();
            mockProcessExcelFormulaToLink.Setup(x => x.GetLinkFromFormula(It.IsAny<string>()))
                .Returns("http://www.test.com/");

            mockOverallEffectivenessProcessor.Setup(x => x.GetOverallEffectiveness(It.IsAny<string>()))
                .Returns(OverallEffectiveness.Outstanding);

            var getOfstedInspections =
                new GetOfstedInspections(mockProcessExcelFormulaToLink.Object, mockOverallEffectivenessProcessor.Object);

            var res = getOfstedInspections.GetAll();


            Assert.IsTrue(true);

        }
    }
}
