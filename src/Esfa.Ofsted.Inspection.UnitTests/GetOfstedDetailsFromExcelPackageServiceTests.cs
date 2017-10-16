using Esfa.Ofsted.Inspection.Client.Services.Interfaces;
using Moq;
using NUnit.Framework;

namespace Esfa.Ofsted.Inspection.UnitTests
{
    [TestFixture]
    public class GetOfstedDetailsFromExcelPackageServiceTests
    {
        [Test]
        public void ShouldReturnResultsWithoutErrors()
        {
            var mockProcessExcelFormulaToLink = new Mock<IProcessExcelFormulaToLink>();
            mockProcessExcelFormulaToLink.Setup(x => x.GetLinkFromFormula(It.IsAny<string>())).Returns(string.Empty);
//            mockAngleSharpService.Setup(x => x.GetLinks(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
//                .Returns(new List<string> { "firstMatchingLink" });
        }
    }
}
