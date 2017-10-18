using System;
using Esfa.Ofsted.Inspection.Client.ApplicationServices;
using Esfa.Ofsted.Inspection.Client.Services;
using Esfa.Ofsted.Inspection.Client.Services.Interfaces;
using Esfa.Ofsted.Inspection.Types;
using Esfa.Ofsted.Inspection.Types.Exceptions;
using Moq;
using NUnit.Framework;

namespace Esfa.Ofsted.Inspection.UnitTests
{
    [TestFixture]
    public class GetInspectionServiceTests
    {

        [Test]
        public void TestAnInvalidUrl()
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

            var getInspectionService = new GetInspectionsService(mockLogger.Object, Mock.Of<IGetOfstedDetailsFromExcelPackageService>());

            var invalidUrl = "abc";
         
            Assert.Throws<UrlReadingException>(() => getInspectionService.GetInspectionsDetail(invalidUrl));

            mockLogger.Verify(x => x.Debug, Times.Exactly(2));
            Assert.IsTrue(errorMessageString.StartsWith("Error whilst trying to read url: ["), "Logged Error message does not contain expected words");
            Assert.IsNotNull(errorException);
            Assert.IsTrue(errorException.Message.StartsWith("Error whilst trying to read url: ["), "Exception message does not contain expected words");
        }

        [Test]
        public void TestALinkThatIsNotExcel()
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

            var getInspectionService = new GetInspectionsService(mockLogger.Object, Mock.Of<IGetOfstedDetailsFromExcelPackageService>());

            var urlWithoutExcel = "http://www.google.com";
      
            Assert.Throws<UrlReadingException>(() => getInspectionService.GetInspectionsDetail(urlWithoutExcel));
            mockLogger.Verify(x => x.Debug, Times.Exactly(3));
            Assert.IsTrue(errorMessageString.StartsWith("Error whilst trying to read excel details from url: ["), "Logged Error message does not contain expected words");
            Assert.IsNotNull(errorException);
            Assert.IsTrue(errorException.Message.StartsWith("Error whilst trying to read excel details from url: ["), "Exception message does not contain expected words");
        }
    }
}
