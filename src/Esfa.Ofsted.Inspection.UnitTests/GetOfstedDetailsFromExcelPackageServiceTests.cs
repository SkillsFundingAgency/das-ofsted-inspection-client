using System.Collections.Generic;
using Esfa.Ofsted.Inspection.Client.Services;
using Esfa.Ofsted.Inspection.Client.Services.Interfaces;
using Esfa.Ofsted.Inspection.Types;
using Moq;
using NUnit.Framework;
using OfficeOpenXml;

namespace Esfa.Ofsted.Inspection.UnitTests
{
    [TestFixture]
    public class GetOfstedDetailsFromExcelPackageServiceTests
    {
        //[Test]
        //public void ShouldReturnResultsWithoutErrors()
        //{
        //    var mockProcessExcelFormulaToLink = new Mock<IProcessExcelFormulaToLink>();
            
        //    mockProcessExcelFormulaToLink.Setup(x => x.GetLinkFromFormula(It.IsAny<string>())).Returns(string.Empty);
        //    mockAngleSharpService.Setup(x => x.GetLinks(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
        //        .Returns(new List<string> { "firstMatchingLink" });
        //}

        [Test]
        public void ShouldErrorAsInvalidExcelPackagePassedIn()
        {
            var excelPackage = new ExcelPackage();
            var getOfstedDetailsFromExcelPackageService
                = new GetOfstedDetailsFromExcelPackageService(Mock.Of<IProcessExcelFormulaToLink>(),
                    Mock.Of<OverallEffectivenessProcessor>(), Mock.Of<IConfigurationSettings>());

            var inspectionDetails = getOfstedDetailsFromExcelPackageService.ExtractOfstedInspections(excelPackage);
            Assert.IsNull(inspectionDetails.Inspections);
            Assert.AreEqual(inspectionDetails.StatusCode, InspectionsStatusCode.NotProcessed);
            Assert.IsNull(inspectionDetails.ErrorSet);
            Assert.IsTrue(inspectionDetails.NotProcessedMessage.StartsWith("No worksheet found in the datasource that matches"));      
        }



    }
}
