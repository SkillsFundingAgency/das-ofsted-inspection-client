using Esfa.Ofsted.Inspection.Client.Services;
using Esfa.Ofsted.Inspection.Client.Services.Interfaces;
using Esfa.Ofsted.Inspection.Types;
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
            var getInspectionService = new GetInspectionsService(Mock.Of<IGetOfstedDetailsFromExcelPackageService>());

            var invalidUrl = "abc";
            var res = getInspectionService.GetInspectionsDetail(invalidUrl);

            Assert.AreEqual(InspectionsStatusCode.NotProcessed, res.StatusCode,
                "The status code returned was not 'NotProcessed'");
            Assert.IsNull(res.Inspections, $"The actual was not the expected null");
            Assert.IsNull(res.ErrorSet, $"The actual errorset was not null, instead of null");
            Assert.IsTrue(res.NotProcessedMessage.StartsWith($"Error whilst trying to read url: [{invalidUrl}], message: ["), "Message returned from error is not as expected");
        }

        [Test]
        public void TestALinkThatIsNotExcel()
        {
            var getInspectionService = new GetInspectionsService(Mock.Of<IGetOfstedDetailsFromExcelPackageService>());

            var urlWithoutExcel = "http://www.google.com";
            var res = getInspectionService.GetInspectionsDetail(urlWithoutExcel);

            Assert.AreEqual(InspectionsStatusCode.NotProcessed, res.StatusCode,
                "The status code returned was not 'NotProcessed'");
            Assert.IsNull(res.Inspections, $"The actual was not the expected null");
            Assert.IsNull(res.ErrorSet, $"The actual errorset was not null, instead of null");
            Assert.IsTrue(res.NotProcessedMessage.StartsWith($"Error whilst trying to read excel details from url: [{urlWithoutExcel}], message: ["), "Message returned from error is not as expected");
        }
    }
}
