using System;
using System.Collections.Generic;
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
    public class GetOfstedInspectionsTests
    {
      
        [Test]
        public void ShouldReturnResultsWithoutErroring()
        {
            var mockAngleSharpService = new Mock<IAngleSharpService>();
            mockAngleSharpService.Setup(x => x.GetLinks(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new List<string> {"firstMatchingLink" });

            var mockConfigurationSettings = new Mock<IConfigurationSettings>();
            mockConfigurationSettings.Setup(x => x.InspectionSiteUrl).Returns("http://www.test.com/test2");
            mockConfigurationSettings.Setup(x => x.LinkText).Returns("linkText Goes Here");

            var mockGetInspectionsService = new Mock<IGetInspectionsService>();

            var inspectionDetail = new InspectionsDetail { StatusCode = InspectionsStatusCode.Success,
                                                           Inspections = new List<OfstedInspection> {new OfstedInspection()},
                                                           ErrorSet = null};

            var mockLogger = new Mock<ILogFunctions>();

            var messageString = string.Empty;
            var action = new Action<string>(message => {messageString = message;});

            mockLogger.SetupGet(x => x.Info).Returns(action);
            mockLogger.SetupGet(x => x.Debug).Returns(action);


            mockGetInspectionsService.Setup(x => x.GetInspectionsDetail(It.IsAny<string>())).Returns(inspectionDetail);

            var getOfstedInspections =
                new GetOfstedInspections(mockLogger.Object,
                                         mockAngleSharpService.Object,
                                         mockConfigurationSettings.Object,
                                         mockGetInspectionsService.Object);

            var res = getOfstedInspections.GetAll();

            mockLogger.VerifyAll();
            mockLogger.Verify(x => x.Info, Times.Exactly(2));
            mockLogger.Verify(x => x.Debug, Times.Exactly(2));

            Assert.AreEqual(InspectionsStatusCode.Success, res.StatusCode, $"The expected status code was success, but actual was [{res.StatusCode}]");
            Assert.IsNotNull(res.Inspections,$"The inspections should have some values, but was null");
            Assert.AreEqual(1, res.Inspections.Count, $"The number of inpections expected as 1, but actual was [{res.Inspections.Count}]");
            Assert.IsNull(res.ErrorSet, $"The Errorset expected was null, but actual was not null");


        }

        [Test]
        public void ShouldGiveNotProcessedErrorAndMessageWhenNoLinksFound()
        {
            var mockAngleSharpService = new Mock<IAngleSharpService>();
            mockAngleSharpService.Setup(x => x.GetLinks(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new List<string>());

            var mockConfigurationSettings = new Mock<IConfigurationSettings>();
            var mockLogger = new Mock<ILogFunctions>();
            var mockGetInspectionsService = new Mock<GetInspectionsService>(mockLogger.Object, Mock.Of<IGetOfstedDetailsFromExcelPackageService>());

            mockConfigurationSettings.Setup(x => x.InspectionSiteUrl).Returns("http://www.test.com/test2");
            mockConfigurationSettings.Setup(x => x.LinkText).Returns("linkText Goes Here");
        
            var errorMessageString = string.Empty;
            Exception errorException = null;
            var errorAction = new Action<string, Exception>((message, exception) =>
            {
                errorMessageString = message;
                errorException = exception;
            });
            mockLogger.SetupGet(x => x.Error).Returns(errorAction);

            var action = new Action<string>(message => { });

            mockLogger.SetupGet(x => x.Info).Returns(action);
            mockLogger.SetupGet(x => x.Debug).Returns(action);

            var getOfstedInspections =
                new GetOfstedInspections(
                    mockLogger.Object,
                    mockAngleSharpService.Object,
                    mockConfigurationSettings.Object,
                    mockGetInspectionsService.Object
                );

           
            Assert.Throws<NoLinksInPageException>(() => getOfstedInspections.GetAll());
            mockLogger.Verify(x => x.Info, Times.Exactly(1));
            mockLogger.Verify(x => x.Debug, Times.Exactly(0));
            Assert.IsTrue(errorMessageString.StartsWith("Could not locate any links in page ["), "Logged Error message does not contain expected words");
            Assert.IsNotNull(errorException);
            Assert.IsTrue(errorException.Message.StartsWith("Could not locate any links in page ["), "Exception message does not contain expected words");
        }

        [Test]
        public void ShouldGiveNotProcessedErrorAndMessageWhenFirstLinkIsNotUrl()
        {
            var mockAngleSharpService = new Mock<IAngleSharpService>();
            mockAngleSharpService.Setup(x => x.GetLinks(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new List<string> {"firstMatchingLink"});

            var mockConfigurationSettings = new Mock<IConfigurationSettings>();
            mockConfigurationSettings.Setup(x => x.InspectionSiteUrl).Returns("badurl");
            var mockLogger = new Mock<ILogFunctions>();
            var mockGetInspectionsService = new Mock<GetInspectionsService>(mockLogger.Object,Mock.Of<IGetOfstedDetailsFromExcelPackageService>());
         
            var errorMessageString = string.Empty;
            Exception errorException = null;
            var errorAction = new Action<string, Exception>((message, exception) => 
                    {
                    errorMessageString = message;
                    errorException = exception;
                    });
            mockLogger.SetupGet(x => x.Error).Returns(errorAction);

            var messageString = string.Empty;
            var action = new Action<string>(message => { messageString = message; });

            mockLogger.SetupGet(x => x.Info).Returns(action);
            mockLogger.SetupGet(x => x.Debug).Returns(action);

            var getOfstedInspections =
                new GetOfstedInspections(
                    mockLogger.Object,
                    mockAngleSharpService.Object,
                    mockConfigurationSettings.Object,
                    mockGetInspectionsService.Object
                );
   
            Assert.Throws<InvalidLinkException>(() => getOfstedInspections.GetAll());
            mockLogger.Verify(x => x.Info, Times.Exactly(1));
            mockLogger.Verify(x => x.Debug, Times.Exactly(1));
            Assert.IsTrue(errorMessageString.StartsWith("Could not build a valid url from url ["), "Logged Error message does not contain expected words");
            Assert.IsNotNull(errorException);
            Assert.IsTrue(errorException.Message.StartsWith("Could not build a valid url from url ["), "Exception message does not contain expected words");
        }
    }
}
