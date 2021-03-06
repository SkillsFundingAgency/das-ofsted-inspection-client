﻿using System;
using System.Globalization;
using System.Linq;
using Esfa.Ofsted.Inspection.Client;
using Esfa.Ofsted.Inspection.Client.Services;
using Esfa.Ofsted.Inspection.Client.Services.Interfaces;
using Esfa.Ofsted.Inspection.Types;
using Esfa.Ofsted.Inspection.Types.Exceptions;
using Moq;
using NUnit.Framework;
using OfficeOpenXml;

namespace Esfa.Ofsted.Inspection.UnitTests
{
    [TestFixture]
    public class GetOfstedDetailsFromExcelPackageServiceTests
    {
        private const string FocusWorksheet = "worksheet with details";

        [Test]
        public void ShouldErrorAsInvalidExcelPackagePassedIn()
        {
            var excelPackage = CreateBasicExcelSpreadsheetForTesting();

            var mockLogger = new Mock<ILogFunctions>();
            var errorMessageString = string.Empty;
            Exception errorException = null;
            var errorAction = new Action<string, Exception>((message, exception) =>
            {
                errorMessageString = message;
                errorException = exception;
            });
            mockLogger.SetupGet(x => x.Error).Returns(errorAction);

            var getOfstedDetailsFromExcelPackageService
                = new GetOfstedDetailsFromExcelPackageService(mockLogger.Object,Mock.Of<IProcessExcelFormulaToLink>(),
                    Mock.Of<OverallEffectivenessProcessor>(), Mock.Of<IConfigurationSettings>());

            Assert.Throws<NoWorksheetPresentException>(() => getOfstedDetailsFromExcelPackageService.ExtractOfstedInspections(excelPackage));
            mockLogger.Verify(x => x.Error, Times.Exactly(1));
            Assert.IsTrue(errorMessageString.StartsWith("No worksheet found in the datasource that matches '"), "Logged Error message does not contain expected words");
            Assert.IsNotNull(errorException);
            Assert.IsTrue(errorException.Message.StartsWith("No worksheet found in the datasource that matches '"), "Exception message does not contain expected words");
        }

        [Test]
        public void ShouldReturnSuccessWithExpectedNumberOfDetailsAndNoErrors()
        {
            const string hyperlink = "=HYPERLINK(\"http://www.ofsted.gov.uk/inspection-reports/find-inspection-report/provider/ELS/54805  \",\"Ofsted Webpage\")";
      
            var excelPackage = CreateBasicExcelSpreadsheetForTesting();
            var excelWorksheet = excelPackage.Workbook.Worksheets[FocusWorksheet];

            CreateRow(excelWorksheet, 5, hyperlink, "10033440", new DateTime(2017,08,31), "9");
            CreateRow(excelWorksheet, 6, "random", "10033441", new DateTime(2017,09,30), "9");

            var mockConfigurationSettings = new Mock<IConfigurationSettings>();
            mockConfigurationSettings.Setup(x => x.WorksheetName).Returns(FocusWorksheet);
            mockConfigurationSettings.Setup(x => x.WebLinkHeading).Returns("Web link");
            mockConfigurationSettings.Setup(x => x.UkPrnHeading).Returns("Provider UKPRN");
            mockConfigurationSettings.Setup(x => x.DatePublishedHeading).Returns("Web link");
            mockConfigurationSettings.Setup(x => x.DatePublishedHeading).Returns("Date published");
            mockConfigurationSettings.Setup(x => x.OverallEffectivenessHeading).Returns("Overall effectiveness");
            var mockProcessExcelFormulaToLink = new Mock<IProcessExcelFormulaToLink>();
            mockProcessExcelFormulaToLink.Setup(x => x.GetLinkFromFormula(It.IsAny<string>(), It.IsAny<string>())).Returns((string)null);

            var mockOverallEffectivenessProcessor = new Mock<IOverallEffectivenessProcessor>();
            mockOverallEffectivenessProcessor.Setup(x => x.GetOverallEffectiveness("9"))
                .Returns(OverallEffectiveness.RemainedGoodAtAShortInspectionThatDidNotConvert);

            var mockLogger = new Mock<ILogFunctions>();
            var action = new Action<string>(message => { });
            mockLogger.SetupGet(x => x.Debug).Returns(action);

            var getOfstedDetailsFromExcelPackageService
                = new GetOfstedDetailsFromExcelPackageService(mockLogger.Object, mockProcessExcelFormulaToLink.Object,
                    mockOverallEffectivenessProcessor.Object, mockConfigurationSettings.Object);

            var inspectionDetails = getOfstedDetailsFromExcelPackageService.ExtractOfstedInspections(excelPackage);

            mockLogger.Verify(x => x.Debug, Times.Exactly(2));
            Assert.AreEqual(2, inspectionDetails.InspectionOutcomes.Count, $"2 inspections were expected, but {inspectionDetails.InspectionOutcomes.Count} was returned");
            Assert.AreEqual(InspectionsStatusCode.Success, inspectionDetails.StatusCode, "InspectionDetails status code was expected to be Success");
            Assert.AreEqual(0, inspectionDetails.InspectionOutcomeErrors.Count, "The Errorset was expected to be 0");
        }

        [Test]
        public void ShouldReturnProcessedWithErrorsWithExpectedNumberOfDetailsAndSomeErrors()
        {
            const string hyperlink = "=HYPERLINK(\"http://www.ofsted.gov.uk/inspection-reports/find-inspection-report/provider/ELS/54805  \",\"Ofsted Webpage\")";
    
            var excelPackage = CreateBasicExcelSpreadsheetForTesting();
            var excelWorksheet = excelPackage.Workbook.Worksheets[FocusWorksheet];


            CreateRow(excelWorksheet, 5, hyperlink, "10033440", new DateTime(2017,08,31), "9");
            CreateRow(excelWorksheet, 6, "random", "10033441", new DateTime(2017, 09, 30), "9");
            CreateRow(excelWorksheet, 7, "random", "", new DateTime(2017, 09, 29), "4");
            CreateRow(excelWorksheet, 8, "random", "10033442", "date goes here", "9");
            CreateRow(excelWorksheet, 9, "random", "10033443", new DateTime(2017, 09, 28), "x");
            CreateRow(excelWorksheet, 10, "random", "", "date stuff", "notvalid");
            CreateRow(excelWorksheet, 11, "random", "10033442", "NULL", "9");
            CreateRow(excelWorksheet, 12, "random", "10033442", "null", "9");
            CreateRow(excelWorksheet, 13, "random", "10033442", "Null", "9");
            CreateRow(excelWorksheet, 14, "random", "10033443", null, "9");
            CreateRow(excelWorksheet, 15, "random", "10033444", "", "9");



            var mockConfigurationSettings = new Mock<IConfigurationSettings>();
            mockConfigurationSettings.Setup(x => x.WorksheetName).Returns(FocusWorksheet);
            mockConfigurationSettings.Setup(x => x.WebLinkHeading).Returns("Web link");
            mockConfigurationSettings.Setup(x => x.UkPrnHeading).Returns("Provider UKPRN");
            mockConfigurationSettings.Setup(x => x.DatePublishedHeading).Returns("Web link");
            mockConfigurationSettings.Setup(x => x.DatePublishedHeading).Returns("Date published");
            mockConfigurationSettings.Setup(x => x.OverallEffectivenessHeading).Returns("Overall effectiveness");
            var mockProcessExcelFormulaToLink = new Mock<IProcessExcelFormulaToLink>();
            mockProcessExcelFormulaToLink.Setup(x => x.GetLinkFromFormula(It.IsAny<string>(),It.IsAny<string>())).Returns((string)null);
 
            var mockOverallEffectivenessProcessor = new Mock<IOverallEffectivenessProcessor>();
            mockOverallEffectivenessProcessor.Setup(x => x.GetOverallEffectiveness("9"))
                .Returns(OverallEffectiveness.RemainedGoodAtAShortInspectionThatDidNotConvert);

            mockOverallEffectivenessProcessor.Setup(x => x.GetOverallEffectiveness("4"))
                .Returns(OverallEffectiveness.Inadequate);

            var mockLogger = new Mock<ILogFunctions>();
            var action = new Action<string>(message => { });
            mockLogger.SetupGet(x => x.Debug).Returns(action);
            mockLogger.SetupGet(x => x.Warn).Returns(action);

            var getOfstedDetailsFromExcelPackageService
                = new GetOfstedDetailsFromExcelPackageService(mockLogger.Object, mockProcessExcelFormulaToLink.Object,
                    mockOverallEffectivenessProcessor.Object, mockConfigurationSettings.Object);

            var inspectionDetails = getOfstedDetailsFromExcelPackageService.ExtractOfstedInspections(excelPackage);


            mockLogger.Verify(x => x.Debug, Times.Exactly(5));
            mockLogger.Verify(x => x.Warn, Times.Exactly(6));

            Assert.Multiple(() =>
            {
                Assert.AreEqual(5, inspectionDetails.InspectionOutcomes.Count,
                    $"5 inspections were expected, but {inspectionDetails.InspectionOutcomes.Count} was returned");
                Assert.AreEqual(InspectionsStatusCode.ProcessedWithErrors, inspectionDetails.StatusCode,
                    "InspectionDetails status code was expected to be Processed with errors");
                Assert.AreEqual(6, inspectionDetails.InspectionOutcomeErrors.Count, "The Errorset was expected to be 6");
                Assert.AreEqual("29/09/2017", inspectionDetails.InspectionOutcomeErrors.Single(x => x.LineNumber == 7).DatePublished);
                Assert.AreEqual(string.Empty, inspectionDetails.InspectionOutcomeErrors.Single(x => x.LineNumber == 7).Ukprn);
                Assert.AreEqual("4", inspectionDetails.InspectionOutcomeErrors.Single(x => x.LineNumber == 7).OverallEffectiveness);
                Assert.AreEqual("10033442", inspectionDetails.InspectionOutcomeErrors.Single(x => x.LineNumber == 8).Ukprn);
                Assert.AreEqual("date goes here", inspectionDetails.InspectionOutcomeErrors.Single(x => x.LineNumber == 8).DatePublished);
                Assert.AreEqual("9", inspectionDetails.InspectionOutcomeErrors.Single(x => x.LineNumber ==8).OverallEffectiveness);
                Assert.AreEqual("10033443", inspectionDetails.InspectionOutcomeErrors.Single(x => x.LineNumber == 9).Ukprn);
                Assert.AreEqual("28/09/2017", inspectionDetails.InspectionOutcomeErrors.Single(x => x.LineNumber == 9).DatePublished);
                Assert.AreEqual("x", inspectionDetails.InspectionOutcomeErrors.Single(x => x.LineNumber == 9).OverallEffectiveness);
                Assert.AreEqual(string.Empty, inspectionDetails.InspectionOutcomeErrors.Single(x => x.LineNumber == 10).Ukprn);
                Assert.AreEqual("date stuff", inspectionDetails.InspectionOutcomeErrors.Single(x => x.LineNumber == 10).DatePublished);
                Assert.AreEqual("notvalid", inspectionDetails.InspectionOutcomeErrors.Single(x => x.LineNumber == 10).OverallEffectiveness);
                Assert.AreEqual("Invalid value for ukprn []; Invalid value for Overall Effectiveness [notvalid]; Invalid value for Date Published [date stuff]; ", inspectionDetails.InspectionOutcomeErrors.Single(x=>x.LineNumber == 10).Message);

            });
        }

        [Test]
        public void ShouldReturnErrorWithNoDetailsAndSomeErrors()
        {
            var excelPackage = CreateBasicExcelSpreadsheetForTesting();
            var excelWorksheet = excelPackage.Workbook.Worksheets[FocusWorksheet];


            CreateRow(excelWorksheet, 5, "random words", "", new DateTime(2017,09,29), "4");
            CreateRow(excelWorksheet, 6, "random", "10033442", "date goes here", "9");
            CreateRow(excelWorksheet, 7, "random", "10033443", new DateTime(2017,09,28), "x");

            var mockConfigurationSettings = new Mock<IConfigurationSettings>();
            mockConfigurationSettings.Setup(x => x.WorksheetName).Returns(FocusWorksheet);
            mockConfigurationSettings.Setup(x => x.WebLinkHeading).Returns("Web link");
            mockConfigurationSettings.Setup(x => x.UkPrnHeading).Returns("Provider UKPRN");
            mockConfigurationSettings.Setup(x => x.DatePublishedHeading).Returns("Web link");
            mockConfigurationSettings.Setup(x => x.DatePublishedHeading).Returns("Date published");
            mockConfigurationSettings.Setup(x => x.OverallEffectivenessHeading).Returns("Overall effectiveness");
            var mockProcessExcelFormulaToLink = new Mock<IProcessExcelFormulaToLink>();
            mockProcessExcelFormulaToLink.Setup(x => x.GetLinkFromFormula(It.IsAny<string>(), It.IsAny<string>())).Returns(string.Empty);
     
            var mockOverallEffectivenessProcessor = new Mock<IOverallEffectivenessProcessor>();
            mockOverallEffectivenessProcessor.Setup(x => x.GetOverallEffectiveness("9"))
                .Returns(OverallEffectiveness.RemainedGoodAtAShortInspectionThatDidNotConvert);

            mockOverallEffectivenessProcessor.Setup(x => x.GetOverallEffectiveness("4"))
                .Returns(OverallEffectiveness.Inadequate);

            var mockLogger = new Mock<ILogFunctions>();
            var action = new Action<string>(message => { });
            mockLogger.SetupGet(x => x.Debug).Returns(action);
            mockLogger.SetupGet(x => x.Warn).Returns(action);

            var errorMessageString = string.Empty;
            Exception errorException = null;
            var errorAction = new Action<string, Exception>((message, exception) =>
            {
                errorMessageString = message;
                errorException = exception;
            });

            mockLogger.SetupGet(x => x.Error).Returns(errorAction);

            var getOfstedDetailsFromExcelPackageService
                = new GetOfstedDetailsFromExcelPackageService(mockLogger.Object,mockProcessExcelFormulaToLink.Object,
                    mockOverallEffectivenessProcessor.Object, mockConfigurationSettings.Object);

            Assert.Throws<MissingInspectionOutcomesException>(() => getOfstedDetailsFromExcelPackageService.ExtractOfstedInspections(excelPackage));
            mockLogger.Verify(x => x.Warn, Times.Exactly(3));   
            mockLogger.Verify(x => x.Error, Times.Exactly(1));
            Assert.IsTrue(errorMessageString.Equals("No inspections were processed successfully"), "Logged Error message does not contain expected words");
            Assert.IsNotNull(errorException);
            Assert.AreEqual(3,errorException.Data.Values.Count); 
            
            var inspectionErrorOnLine5 = (InspectionError)errorException.Data["5"];
            var inspectionErrorOnLine7 = (InspectionError)errorException.Data["7"];
            Assert.AreEqual(string.Empty, inspectionErrorOnLine5.Website);   
            Assert.AreEqual("", inspectionErrorOnLine5.Ukprn);
            Assert.AreEqual(new DateTime(2017, 09, 29).ToString("dd/MM/yyyy",CultureInfo.InvariantCulture), inspectionErrorOnLine5.DatePublished);   
            Assert.AreEqual("4", inspectionErrorOnLine5.OverallEffectiveness);
            Assert.IsNotEmpty(inspectionErrorOnLine5.Message);

            Assert.AreEqual(string.Empty, inspectionErrorOnLine7.Website);
            Assert.AreEqual("10033443", inspectionErrorOnLine7.Ukprn);
            Assert.AreEqual(new DateTime(2017, 09, 28).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture), inspectionErrorOnLine7.DatePublished);
            Assert.AreEqual("x", inspectionErrorOnLine7.OverallEffectiveness);
            Assert.IsNotEmpty(inspectionErrorOnLine7.Message);
    
            Assert.IsTrue(errorException.Message.Equals("No inspections were processed successfully"), "Exception message does not contain expected words");
        }

        private static ExcelPackage CreateBasicExcelSpreadsheetForTesting()
        {
            var excelPackage = new ExcelPackage();
            var configurationSettings = new ConfigurationSettings();

            excelPackage.Workbook.Worksheets.Add("worksheet 1");
            var excelWorksheet = excelPackage.Workbook.Worksheets.Add(FocusWorksheet);
            excelWorksheet.Cells[1, 1].Value =
                "In-year full and short inspection outcomes for further education and skills providers";
            excelWorksheet.Cells[4, 1].Value = configurationSettings.WebLinkHeading;
            excelWorksheet.Cells[4, 3].Value = configurationSettings.UkPrnHeading;
            excelWorksheet.Cells[4, 16].Value = configurationSettings.DatePublishedHeading;
            excelWorksheet.Cells[4, 17].Value = configurationSettings.OverallEffectivenessHeading;
            return excelPackage;
        }

        private static void CreateRow(ExcelWorksheet excelWorksheet, int rowNumber, string url, string ukprn, object datePublished, string overallEffectiveness)
        {
            excelWorksheet.Cells[rowNumber, 1].Formula = url;
            excelWorksheet.Cells[rowNumber, 3].Value = ukprn;
            excelWorksheet.Cells[rowNumber, 16].Value = datePublished;
            excelWorksheet.Cells[rowNumber, 17].Value = overallEffectiveness;
        }
    }
}
