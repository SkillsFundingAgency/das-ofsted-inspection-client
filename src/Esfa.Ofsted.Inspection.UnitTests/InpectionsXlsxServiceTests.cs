using System.Collections.Generic;
using Esfa.Ofsted.Inspection.Client.ApplicationServices;
using Esfa.Ofsted.Inspection.Client.Services.interfaces;
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
            var mockAngleSharpService = new Mock<IAngleSharpService>();
            mockProcessExcelFormulaToLink.Setup(x => x.GetLinkFromFormula(It.IsAny<string>()))
                .Returns("http://www.test.com/");

            mockOverallEffectivenessProcessor.Setup(x => x.GetOverallEffectiveness(It.IsAny<string>()))
                .Returns(OverallEffectiveness.Outstanding);

            mockAngleSharpService.Setup(x => x.GetLinks(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new List<string>
                    {
                    "/government/uploads/system/uploads/attachment_data/file/643394/Management_information_-_further_education_and_skills_-_as_at_31_August_2017.xlsx"
                    });

        var getOfstedInspections =
                new GetOfstedInspections(mockProcessExcelFormulaToLink.Object, mockOverallEffectivenessProcessor.Object, mockAngleSharpService.Object);

            var res = getOfstedInspections.GetAll();


            Assert.IsTrue(true);

        }
    }
}
