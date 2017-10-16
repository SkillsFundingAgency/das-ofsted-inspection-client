using System.Collections.Generic;
using Esfa.Ofsted.Inspection.Client.ApplicationServices;
using Esfa.Ofsted.Inspection.Client.Services;
using Esfa.Ofsted.Inspection.Client.Services.Interfaces;
using Esfa.Ofsted.Inspection.Types;
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

            var mockAppSettingsService = new Mock<IConfigurationSettings>();
            mockAppSettingsService.Setup(x => x.InspectionSiteUrl).Returns("http://www.test.com/test2");
            mockAppSettingsService.Setup(x => x.LinkText).Returns("linkText Goes Here");

            var mockGetInspectionsService = new Mock<IGetInspectionsService>();

            var inspectionDetail = new InspectionsDetail { StatusCode = InspectionsStatusCode.Success, Inspections = new List<OfstedInspection> {new OfstedInspection()}, ErrorSet = null};

            mockGetInspectionsService.Setup(x => x.GetInspectionsDetail(It.IsAny<string>())).Returns(inspectionDetail);

            var getOfstedInspections =
                new GetOfstedInspections(mockAngleSharpService.Object,
                    mockAppSettingsService.Object,
                    mockGetInspectionsService.Object);

            var res = getOfstedInspections.GetAll();

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

            var mockAppSettingsService = new Mock<IConfigurationSettings>();
            var mockGetInspectionsService = new Mock<GetInspectionsService>(Mock.Of<IGetOfstedDetailsFromExcelPackageService>());

            mockAppSettingsService.Setup(x => x.InspectionSiteUrl).Returns("http://www.test.com/test2");
            mockAppSettingsService.Setup(x => x.LinkText).Returns("linkText Goes Here");
            var getOfstedInspections =
                new GetOfstedInspections(mockAngleSharpService.Object,
                    mockAppSettingsService.Object,
                    mockGetInspectionsService.Object);

            var res = getOfstedInspections.GetAll();

            Assert.AreEqual(InspectionsStatusCode.NotProcessed, res.StatusCode,
                "The status code returned was not 'NotProcessed'");
            Assert.IsNull(res.Inspections, $"The actual was not the expected null");
            Assert.IsNull(res.ErrorSet, $"The actual errorset was not null, instead of null");
            Assert.IsTrue(res.NotProcessedMessage.StartsWith("Could not locate any links in page"), 
                        $"The actual message didn't contain the expected words. Message was: [{res.NotProcessedMessage}]");
        }

        [Test]
        public void ShouldGiveNotProcessedErrorAndMessageWhenFirstLinkIsNotUrl()
        {
            var mockAngleSharpService = new Mock<IAngleSharpService>();
            mockAngleSharpService.Setup(x => x.GetLinks(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new List<string> {"firstMatchingLink"});

            var mockAppSettingsService = new Mock<IConfigurationSettings>();
            mockAppSettingsService.Setup(x => x.InspectionSiteUrl).Returns("badurl");
        
            var getOfstedInspections =
                new GetOfstedInspections(mockAngleSharpService.Object,
                    mockAppSettingsService.Object,
                    null
                );

            var res = getOfstedInspections.GetAll();

            Assert.AreEqual(InspectionsStatusCode.NotProcessed, res.StatusCode, "The status code returned was not 'NotProcessed'");
            Assert.IsNull(res.Inspections, $"The actual inspections was not the expected null");
            Assert.IsNull(res.ErrorSet, $"The actual errorset was null, instead of not null");
            Assert.IsTrue(res.NotProcessedMessage.StartsWith("Could not build a valid url from url"), 
                                $"The actual message didn't contain the expected words. Message was: [{res.NotProcessedMessage}]");
        }
    }
}
