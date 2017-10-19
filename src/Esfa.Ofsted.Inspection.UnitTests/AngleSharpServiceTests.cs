using System.Linq;
using Esfa.Ofsted.Inspection.Client.Services;
using Esfa.Ofsted.Inspection.Client.Services.Interfaces;
using Moq;
using NUnit.Framework;

namespace Esfa.Ofsted.Inspection.UnitTests
{
    [TestFixture]
    public class AngleSharpServiceTests
    {
        private const string HtmlText = "<html><body>" + "<div>" + "<a href=\"goodbye.com\"> </a>" + "<a href=\"goodbye.com\">Goodbye</a>" + "<a href=\"hello.com\">hELLO</a>" + "<a href=\"hello2.com\">HELLO AGAIN</a>" + "<a href=\"Hej.com\">Hej</a>" + "</div>" + "</body></html>";

        [Test]
        public void WhenGettingLinks()
        {
            var mockHttpGet = new Mock<IHttpGet>();
            mockHttpGet.Setup(m => m.Get(It.IsAny<string>())).Returns(HtmlText);

            var angleSharpService = new AngleSharpService(mockHttpGet.Object);
            var x = angleSharpService.GetLinks("path/to/something", "a", "HELLO");

            Assert.AreEqual(2, x.Count, $"The number of links should be 2, but is {x.Count}");
            Assert.AreEqual("hello.com", x.FirstOrDefault(),"The first link is not to hello.com");
        }

        [Test]
        public void WhenUrlIsEmptyItShouldReturnNoLinks()
        {
            var mockHttpGet = new Mock<IHttpGet>();
            mockHttpGet.Setup(m => m.Get(It.IsAny<string>())).Returns(HtmlText);
            var angleSharpService = new AngleSharpService(mockHttpGet.Object);
            var x = angleSharpService.GetLinks(string.Empty, "a", "HELLO");

            Assert.AreEqual(0, x.Count);
        }
    }
}
