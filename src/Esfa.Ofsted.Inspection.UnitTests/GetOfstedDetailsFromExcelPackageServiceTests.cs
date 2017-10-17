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
        //public void ShouldErrorAsInvalidExcelPackagePassedIn()
        //{
        //    var excelPackage = CreateBasicExcelSpreadsheetForTesting();
        //    var getOfstedDetailsFromExcelPackageService
        //        = new GetOfstedDetailsFromExcelPackageService(Mock.Of<IProcessExcelFormulaToLink>(),
        //            Mock.Of<OverallEffectivenessProcessor>(), Mock.Of<IConfigurationSettings>());

        //    var inspectionDetails = getOfstedDetailsFromExcelPackageService.ExtractOfstedInspections(excelPackage);
        //    Assert.AreEqual(0, inspectionDetails.Inspections.Count);
        //    Assert.AreEqual(InspectionsStatusCode.NotProcessed, inspectionDetails.StatusCode);
        //    Assert.AreEqual(0, inspectionDetails.ErrorSet.Count);
        //    Assert.IsTrue(inspectionDetails.NotProcessedMessage.StartsWith("No worksheet found in the datasource that matches"));      
        //}

        //[Test]
        //public void ShouldErrorAsInvalidIfLineNumberStartNotFound()
        //{
        //    var excelPackage = CreateBasicExcelSpreadsheetForTesting();

        //    var hyperlink =
        //        "=HYPERLINK(\"http://www.ofsted.gov.uk/inspection-reports/find-inspection-report/provider/ELS/54805  \",\"Ofsted Webpage\")";
  
        //    const string focusWorksheet = "worksheet with details";
        //    var mockConfigurationSettings = new Mock<IConfigurationSettings>();
        //    mockConfigurationSettings.Setup(x => x.WorksheetName).Returns(focusWorksheet);

        //    var getOfstedDetailsFromExcelPackageService
        //        = new GetOfstedDetailsFromExcelPackageService(Mock.Of<IProcessExcelFormulaToLink>(),
        //            Mock.Of<IOverallEffectivenessProcessor>(), mockConfigurationSettings.Object);

        //    var excelWorksheet = excelPackage.Workbook.Worksheets[2];
        //    CreateRow(excelWorksheet, 5, hyperlink, string.Empty,string.Empty,"abc");
        //    CreateRow(excelWorksheet, 6, "random", string.Empty, string.Empty, "zed");

        //    var inspectionDetails = getOfstedDetailsFromExcelPackageService.ExtractOfstedInspections(excelPackage);

        //    Assert.AreEqual(0,inspectionDetails.Inspections.Count);
        //    Assert.AreEqual(InspectionsStatusCode.NotProcessed, inspectionDetails.StatusCode);
        //    Assert.AreEqual(0,inspectionDetails.ErrorSet.Count);
        //    Assert.IsTrue(inspectionDetails.NotProcessedMessage.Equals("No details could be found when processing"));
        //}
        

        //[Test]
        //public void ShouldReturnSuccessWithExpectedNumberOfDetailsAndNoErrors()
        //{
        //    var hyperlink =
        //        "=HYPERLINK(\"http://www.ofsted.gov.uk/inspection-reports/find-inspection-report/provider/ELS/54805  \",\"Ofsted Webpage\")";
        //    var hyperlinkResult = @"http://www.ofsted.gov.uk/inspection-reports/find-inspection-report/provider/ELS/54805";

        //    const string focusWorksheet = "worksheet with details";
           
        //    var excelPackage = CreateBasicExcelSpreadsheetForTesting();
        //    var excelWorksheet = excelPackage.Workbook.Worksheets[2];

        //    CreateRow(excelWorksheet, 5, hyperlink, "10033440", "31-08-2017", "9");
        //    CreateRow(excelWorksheet, 6, "random", "10033441", "30-09-2017", "9");

        //    var mockConfigurationSettings = new Mock<IConfigurationSettings>();
        //    mockConfigurationSettings.Setup(x => x.WorksheetName).Returns(focusWorksheet);

        //    var mockProcessExcelFormulaToLink = new Mock<IProcessExcelFormulaToLink>();
        //    mockProcessExcelFormulaToLink.Setup(x => x.GetLinkFromFormula(It.IsAny<string>())).Returns((string)null);
        //    mockProcessExcelFormulaToLink.Setup(x => x.GetLinkFromFormula(hyperlink)).Returns(hyperlinkResult);
            
        //    var mockOverallEffectivenessProcessor = new Mock<IOverallEffectivenessProcessor>();
        //    mockOverallEffectivenessProcessor.Setup(x => x.GetOverallEffectiveness("9"))
        //        .Returns(OverallEffectiveness.RemainedGoodAtAShortInspectionThatDidNotConvert);

        //    var getOfstedDetailsFromExcelPackageService
        //        = new GetOfstedDetailsFromExcelPackageService(mockProcessExcelFormulaToLink.Object,
        //            mockOverallEffectivenessProcessor.Object, mockConfigurationSettings.Object);

        //    var inspectionDetails = getOfstedDetailsFromExcelPackageService.ExtractOfstedInspections(excelPackage);

        //    Assert.AreEqual(2,inspectionDetails.Inspections.Count, $"2 inspections were expected, but {inspectionDetails.Inspections.Count} was returned");
        //    Assert.AreEqual(InspectionsStatusCode.Success,  inspectionDetails.StatusCode,  "InspectionDetails status code was expected to be Success");
        //    Assert.AreEqual(0, inspectionDetails.ErrorSet.Count, "The Errorset was expected to be 0");
        //    Assert.IsNull(inspectionDetails.NotProcessedMessage, "The NotProcessedMessage was expected to be null");
        //}

        [Test]
        public void ShouldReturnProcessedWithErrorsWithExpectedNumberOfDetailsAndSomeErrors()
        {
            var hyperlink =
                "=HYPERLINK(\"http://www.ofsted.gov.uk/inspection-reports/find-inspection-report/provider/ELS/54805  \",\"Ofsted Webpage\")";
            var hyperlinkResult = @"http://www.ofsted.gov.uk/inspection-reports/find-inspection-report/provider/ELS/54805";


            var excelPackage = new ExcelPackage();

            const string focusWorksheet = "worksheet with details";
            excelPackage.Workbook.Worksheets.Add("worksheet 1");
            var excelWorksheet = excelPackage.Workbook.Worksheets.Add(focusWorksheet);
            excelWorksheet.Cells[1, 1].Value =
                "In-year full and short inspection outcomes for further education and skills providers";
            excelWorksheet.Cells[4, 1].Value = "Web link";
            excelWorksheet.Cells[4, 3].Value = "Provider UKPRN";
            excelWorksheet.Cells[4, 16].Value = "Date published";
            excelWorksheet.Cells[4, 17].Value = "Overall effectiveness";
            excelWorksheet = excelPackage.Workbook.Worksheets[2];

            CreateRow(excelWorksheet, 5, hyperlink, "10033440", "31-08-2017", "9");
            CreateRow(excelWorksheet, 6, "random", "10033441", "30-09-2017", "9");
            CreateRow(excelWorksheet,7, "random", "", "29-09-2017", "4");
            CreateRow(excelWorksheet, 8, "random", "10033442", "date goes here", "9");
            CreateRow(excelWorksheet, 9, "random", "10033443", "28-09-2017", "x");
            CreateRow(excelWorksheet, 10, "random", "", "date stuff", "notvalid");


            var mockConfigurationSettings = new Mock<IConfigurationSettings>();
            mockConfigurationSettings.Setup(x => x.WorksheetName).Returns(focusWorksheet);

            var mockProcessExcelFormulaToLink = new Mock<IProcessExcelFormulaToLink>();
            mockProcessExcelFormulaToLink.Setup(x => x.GetLinkFromFormula(It.IsAny<string>())).Returns((string)null);
            mockProcessExcelFormulaToLink.Setup(x => x.GetLinkFromFormula(hyperlink)).Returns(hyperlinkResult);

            var mockOverallEffectivenessProcessor = new Mock<IOverallEffectivenessProcessor>();
            mockOverallEffectivenessProcessor.Setup(x => x.GetOverallEffectiveness("9"))
                .Returns(OverallEffectiveness.RemainedGoodAtAShortInspectionThatDidNotConvert);

            mockOverallEffectivenessProcessor.Setup(x => x.GetOverallEffectiveness("4"))
                .Returns(OverallEffectiveness.Inadequate);

            var getOfstedDetailsFromExcelPackageService
                = new GetOfstedDetailsFromExcelPackageService(mockProcessExcelFormulaToLink.Object,
                    mockOverallEffectivenessProcessor.Object, mockConfigurationSettings.Object);

            var inspectionDetails = getOfstedDetailsFromExcelPackageService.ExtractOfstedInspections(excelPackage);

            Assert.Multiple(() =>
                {
                    //Assert.AreEqual(2, inspectionDetails.Inspections.Count, $"2 inspections were expected, but {inspectionDetails.Inspections.Count} was returned");
                    Assert.AreEqual(InspectionsStatusCode.ProcessedWithErrors, inspectionDetails.StatusCode,
                        "InspectionDetails status code was expected to be Success");
                    Assert.AreEqual(4, inspectionDetails.ErrorSet.Count, "The Errorset was expected to be 4");
                    Assert.AreEqual(string.Empty, inspectionDetails.ErrorSet[0].Ukprn);
                    Assert.AreEqual("29-09-2017", inspectionDetails.ErrorSet[0].DatePublished);
                    Assert.AreEqual("4", inspectionDetails.ErrorSet[0].OverallEffectiveness);
                    Assert.AreEqual("10033442", inspectionDetails.ErrorSet[1].Ukprn);
                    Assert.AreEqual("date goes here", inspectionDetails.ErrorSet[1].DatePublished);
                    Assert.AreEqual("9", inspectionDetails.ErrorSet[1].OverallEffectiveness);
                    Assert.AreEqual("10033443", inspectionDetails.ErrorSet[2].Ukprn);
                    Assert.AreEqual("28-09-2017", inspectionDetails.ErrorSet[2].DatePublished);
                    Assert.AreEqual("x", inspectionDetails.ErrorSet[2].OverallEffectiveness);
                    Assert.AreEqual(string.Empty, inspectionDetails.ErrorSet[3].Ukprn);
                    Assert.AreEqual("date stuff", inspectionDetails.ErrorSet[3].DatePublished);
                    Assert.AreEqual("notvalid", inspectionDetails.ErrorSet[3].OverallEffectiveness);
                    Assert.IsNull(inspectionDetails.NotProcessedMessage, "The NotProcessedMessage was expected to be null");
                }
            );


        }

        //[Test]
        //public void ShouldReturnErrorWithNoDetailsAndSomeErrors()
        //{
        //    var hyperlink =
        //        "=HYPERLINK(\"http://www.ofsted.gov.uk/inspection-reports/find-inspection-report/provider/ELS/54805  \",\"Ofsted Webpage\")";
        //    var hyperlinkResult = @"http://www.ofsted.gov.uk/inspection-reports/find-inspection-report/provider/ELS/54805";

        //    var excelPackage = new ExcelPackage();

        //    const string focusWorksheet = "worksheet with details";
        //    excelPackage.Workbook.Worksheets.Add("worksheet 1");
        //    var excelWorksheet = excelPackage.Workbook.Worksheets.Add(focusWorksheet);
        //    excelWorksheet.Cells[1, 1].Value =
        //        "In-year full and short inspection outcomes for further education and skills providers";
        //    excelWorksheet.Cells[4, 1].Value = "Web link";
        //    excelWorksheet.Cells[4, 3].Value = "Provider UKPRN";
        //    excelWorksheet.Cells[4, 16].Value = "Date published";
        //    excelWorksheet.Cells[4, 17].Value = "Overall effectiveness";
        //    excelWorksheet = excelPackage.Workbook.Worksheets[2];

        //    CreateRow(excelWorksheet, 7, "random", "", "29-09-2017", "4");
        //    CreateRow(excelWorksheet, 8, "random", "10033442", "date goes here", "9");
        //    CreateRow(excelWorksheet, 9, "random", "10033443", "28-09-2017", "x");

        //    var mockConfigurationSettings = new Mock<IConfigurationSettings>();
        //    mockConfigurationSettings.Setup(x => x.WorksheetName).Returns(focusWorksheet);

        //    var mockProcessExcelFormulaToLink = new Mock<IProcessExcelFormulaToLink>();
        //    mockProcessExcelFormulaToLink.Setup(x => x.GetLinkFromFormula(It.IsAny<string>())).Returns((string)null);
        //    mockProcessExcelFormulaToLink.Setup(x => x.GetLinkFromFormula(hyperlink)).Returns(hyperlinkResult);

        //    var mockOverallEffectivenessProcessor = new Mock<IOverallEffectivenessProcessor>();
        //    mockOverallEffectivenessProcessor.Setup(x => x.GetOverallEffectiveness("9"))
        //        .Returns(OverallEffectiveness.RemainedGoodAtAShortInspectionThatDidNotConvert);

        //    mockOverallEffectivenessProcessor.Setup(x => x.GetOverallEffectiveness("4"))
        //        .Returns(OverallEffectiveness.Inadequate);

        //    var getOfstedDetailsFromExcelPackageService
        //        = new GetOfstedDetailsFromExcelPackageService(mockProcessExcelFormulaToLink.Object,
        //            mockOverallEffectivenessProcessor.Object, mockConfigurationSettings.Object);

        //    var inspectionDetails = getOfstedDetailsFromExcelPackageService.ExtractOfstedInspections(excelPackage);

        //    Assert.AreEqual(0, inspectionDetails.Inspections.Count, $"0 inspections were expected, but {inspectionDetails.Inspections.Count} was returned");
        //    Assert.AreEqual(InspectionsStatusCode.NotProcessed, inspectionDetails.StatusCode, "InspectionDetails status code was expected to be NotProcessed");
        //   // Assert.AreEqual(3, inspectionDetails.ErrorSet.Count, "The Errorset was expected to be 3");
        //    Assert.AreEqual(string.Empty, inspectionDetails.ErrorSet[0].Ukprn);
        //    Assert.AreEqual("29-09-2017", inspectionDetails.ErrorSet[0].DatePublished);
        //    Assert.AreEqual("4", inspectionDetails.ErrorSet[0].OverallEffectiveness);
        //    Assert.AreEqual("10033442", inspectionDetails.ErrorSet[1].Ukprn);
        //    Assert.AreEqual("date goes here", inspectionDetails.ErrorSet[1].DatePublished);
        //    Assert.AreEqual("9", inspectionDetails.ErrorSet[1].OverallEffectiveness);
        //    Assert.AreEqual("10033443", inspectionDetails.ErrorSet[2].Ukprn);
        //    Assert.AreEqual("28-09-2017", inspectionDetails.ErrorSet[2].DatePublished);
        //    Assert.AreEqual("x", inspectionDetails.ErrorSet[2].OverallEffectiveness);
        //    Assert.AreEqual("No inspections were processed successfully", inspectionDetails.NotProcessedMessage);
        //}

        private ExcelPackage CreateBasicExcelSpreadsheetForTesting()
        {
            var excelPackage = new ExcelPackage();

            const string focusWorksheet = "worksheet with details";
            excelPackage.Workbook.Worksheets.Add("worksheet 1");
            var excelWorksheet = excelPackage.Workbook.Worksheets.Add(focusWorksheet);
            excelWorksheet.Cells[1, 1].Value =
                "In-year full and short inspection outcomes for further education and skills providers";
            excelWorksheet.Cells[4, 1].Value = "Web link";
            excelWorksheet.Cells[4, 3].Value = "Provider UKPRN";
            excelWorksheet.Cells[4, 16].Value = "Date published";
            excelWorksheet.Cells[4, 17].Value = "Overall effectiveness";
            return excelPackage;
        }

        private static void CreateRow(ExcelWorksheet excelWorksheet, int rowNumber, string url, string ukprn, string datePublished, string overallEffectiveness)
        {
            excelWorksheet.Cells[rowNumber, 1].Formula = url;
            excelWorksheet.Cells[rowNumber, 3].Value = ukprn;
            excelWorksheet.Cells[rowNumber, 16].Value = datePublished;
            excelWorksheet.Cells[rowNumber, 17].Value = overallEffectiveness;
        }

    }
}
