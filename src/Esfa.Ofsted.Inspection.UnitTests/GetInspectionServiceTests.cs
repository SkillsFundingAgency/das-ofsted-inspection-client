using System;
using Esfa.Ofsted.Inspection.Client;
using Esfa.Ofsted.Inspection.Client.Services;
using Esfa.Ofsted.Inspection.Client.Services.Interfaces;
using Esfa.Ofsted.Inspection.Types.Exceptions;
using Moq;
using NUnit.Framework;
using OfficeOpenXml;

namespace Esfa.Ofsted.Inspection.UnitTests
{
    [TestFixture]
    public class GetInspectionServiceTests
    {
        private const string FocusWorksheet = "worksheet with details";

        [TestCase("abc", "Error whilst trying to read url: [")]
        [TestCase("", "Error whilst trying to read url: [")]
        [TestCase(null, "Error whilst trying to read excel details")]
        public void TestAnInvalidUrlForCorrectException(string invalidUrl, string exceptionMessage)
        {
            var mockLogger = new Mock<ILogFunctions>();
            var errorMessageString = string.Empty;
            Exception errorException = null;
            var errorAction = new Action<string, Exception>((message, exception) =>
            {
                errorMessageString = message;
                errorException = exception;
            });
            mockLogger.SetupGet(x => x.Error).Returns(errorAction);

            var action = new Action<string>(message => { });
            mockLogger.SetupGet(x => x.Debug).Returns(action);

            var mockWebClientFactory = new Mock<IWebClientFactory>();
            var mockWebClient = new Mock<IWebClient>();
            mockWebClient.Setup(x => x.DownloadData(It.IsAny<Uri>())).Throws<UriFormatException>();

            mockWebClientFactory.Setup(x => x.Create()).Returns(mockWebClient.Object);

            var getInspectionService = new GetInspectionsService(mockLogger.Object, Mock.Of<IGetOfstedDetailsFromExcelPackageService>(), mockWebClientFactory.Object);
       
            Assert.Throws<UrlReadingException>(() => getInspectionService.GetInspectionsDetail(invalidUrl));

            mockLogger.Verify(x => x.Debug, Times.Exactly(2));
            Assert.IsTrue(errorMessageString.StartsWith(exceptionMessage), "Logged Error message does not contain expected words");
            Assert.IsNotNull(errorException);
            Assert.IsTrue(errorException.Message.StartsWith(exceptionMessage), "Exception message does not contain expected words");
        }

        [Test]
        public void TestALinkThatIsNotExcelForCorrectException()
        {
            var mockLogger = new Mock<ILogFunctions>();
            var errorMessageString = string.Empty;
            Exception errorException = null;
            var errorAction = new Action<string, Exception>((message, exception) =>
            {
                errorMessageString = message;
                errorException = exception;
            });
            mockLogger.SetupGet(x => x.Error).Returns(errorAction);
            var action = new Action<string>(message => { });

            mockLogger.SetupGet(x => x.Debug).Returns(action);

            var mockWebClientFactory = new Mock<IWebClientFactory>();
            var mockWebClient = new Mock<IWebClient>();
            mockWebClient.Setup(x => x.DownloadData(It.IsAny<Uri>())).Returns(new byte[20]);
            mockWebClientFactory.Setup(x => x.Create()).Returns(mockWebClient.Object);
            
            var getInspectionService = new GetInspectionsService(mockLogger.Object, Mock.Of<IGetOfstedDetailsFromExcelPackageService>(), mockWebClientFactory.Object);
 
            var urlWithoutExcel = "http://blah.xyz";

            Assert.Throws<UrlReadingException>(() => getInspectionService.GetInspectionsDetail(urlWithoutExcel));
            mockLogger.Verify(x => x.Debug, Times.Exactly(3));
            Assert.IsTrue(errorMessageString.StartsWith("Error whilst trying to read excel details from url: ["), "Logged Error message does not contain expected words");
            Assert.IsNotNull(errorException);
            Assert.IsTrue(errorException.Message.StartsWith("Error whilst trying to read excel details from url: ["), "Exception message does not contain expected words");
        }


        [Test]
        public void TestALinkThatReturnsExcelAndCountLoggerDebugCalls()
        {
            var mockLogger = new Mock<ILogFunctions>();
          
            var action = new Action<string>(message => { });

            mockLogger.SetupGet(x => x.Debug).Returns(action);

            var mockWebClientFactory = new Mock<IWebClientFactory>();
            var mockWebClient = new Mock<IWebClient>();

            var excelPackage = CreateBasicExcelSpreadsheetForStubbing();
           
            mockWebClient.Setup(x => x.DownloadData(It.IsAny<Uri>())).Returns(excelPackage.GetAsByteArray);
            mockWebClientFactory.Setup(x => x.Create()).Returns(mockWebClient.Object);

            var getInspectionService = new GetInspectionsService(mockLogger.Object, Mock.Of<IGetOfstedDetailsFromExcelPackageService>(), mockWebClientFactory.Object);

            const string urlWithExcel = "http://blah.xyz";

            getInspectionService.GetInspectionsDetail(urlWithExcel);
            mockLogger.Verify(x => x.Debug, Times.Exactly(7));
        }

        private ExcelPackage CreateBasicExcelSpreadsheetForStubbing()
        {
            var excelPackage = new ExcelPackage();
            const string hyperlink = "=HYPERLINK(\"http://www.ofsted.gov.uk/inspection-reports/find-inspection-report/provider/ELS/54805  \",\"Ofsted Webpage\")";

            excelPackage.Workbook.Worksheets.Add("worksheet 1");
            var excelWorksheet = excelPackage.Workbook.Worksheets.Add(FocusWorksheet);
            excelWorksheet.Cells[1, 1].Value =
                "In-year full and short inspection outcomes for further education and skills providers";
            excelWorksheet.Cells[4, 1].Value = "Web link";
            excelWorksheet.Cells[4, 3].Value = "Provider UKPRN";
            excelWorksheet.Cells[4, 16].Value = "Date published";
            excelWorksheet.Cells[4, 17].Value = "Overall effectiveness";
            excelWorksheet.Cells[5, 1].Formula = hyperlink;
            excelWorksheet.Cells[5, 3].Value = "10033440";
            excelWorksheet.Cells[5, 16].Value = new DateTime(2017, 08, 31);
            excelWorksheet.Cells[5, 17].Value = "9";
            return excelPackage;
        }

    }
}
