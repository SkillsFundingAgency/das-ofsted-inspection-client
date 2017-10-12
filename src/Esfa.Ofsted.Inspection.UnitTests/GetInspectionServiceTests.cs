using Esfa.Ofsted.Inspection.Client.Services;
using Esfa.Ofsted.Inspection.Client.Services.Interfaces;
using Moq;
using NUnit.Framework;
using Esfa.Ofsted.Inspection.Types;

namespace Esfa.Das.Ofsted.Inspection.UnitTests
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
            Assert.IsNotNull(res.ErrorSet, $"The actual errorset was null, instead of not null");
            Assert.AreEqual(1, res.ErrorSet.Count, $"The actual number of errors was {res.ErrorSet.Count}, instead of the expected 1");
            Assert.AreEqual(0, res.ErrorSet[0].LineNumber, $"The actual line number from the error was {res.ErrorSet[0].LineNumber}, instead of the expected 0");
            Assert.IsTrue(res.ErrorSet[0].Message.StartsWith($"Error whilst trying to read url: [{invalidUrl}], message: ["), "Message returned from error is not as expected");
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
            Assert.IsNotNull(res.ErrorSet, $"The actual errorset was null, instead of not null");
            Assert.AreEqual(1, res.ErrorSet.Count, $"The actual number of errors was {res.ErrorSet.Count}, instead of the expected 1");
            Assert.AreEqual(0, res.ErrorSet[0].LineNumber, $"The actual line number from the error was {res.ErrorSet[0].LineNumber}, instead of the expected 0");
            Assert.IsTrue(res.ErrorSet[0].Message.StartsWith($"Error whilst trying to read excel details from url: [{urlWithoutExcel}], message: ["), "Message returned from error is not as expected");
        }
    }
}
