using Esfa.Ofsted.Inspection.Client.Services;
using NUnit.Framework;

namespace Esfa.Ofsted.Inspection.UnitTests
{
    [TestFixture]
    public class ProcessExcelFormulaToLinkTests
    {

        [TestCase("HYPERLINK(\"http://www.ofsted.gov.uk/  \",\"Ofsted Webpage\")", "Ofsted Webpage", "http://www.ofsted.gov.uk/")]
        [TestCase("", "", "")]
        [TestCase(null, null, "")]
        [TestCase(null, "", "")]
        [TestCase("",null, "")]
        [TestCase(null, "http://www.ofsted.gov.uk/", "http://www.ofsted.gov.uk/")]
        [TestCase("", "http://www.ofsted.gov.uk/", "http://www.ofsted.gov.uk/")]
        [TestCase("", " http://www.ofsted.gov.uk/", "http://www.ofsted.gov.uk/")]
        [TestCase("", " http://www.ofsted.gov.uk/ ", "http://www.ofsted.gov.uk/")]
        [TestCase("HYPERLINK(\"  http://www.ofsted.gov.uk/  \",\"Ofsted Webpage\")", "", "http://www.ofsted.gov.uk/")]
        [TestCase("HYPERLINK(\"  www.ofsted.gov.uk/  \",\"Ofsted Webpage\")", "", "www.ofsted.gov.uk/")]
        [TestCase("HYPERLINK(\"\",\"Ofsted Webpage\")", "http://www.ofsted.gov.uk/", "http://www.ofsted.gov.uk/")]
        [TestCase("", "www.ofsted.gov.uk", "www.ofsted.gov.uk")]
        [TestCase("", "hello there", "hello there")]
        public void ShouldReturnStringModifiedForUrlUsage(string formula, string cellText, string encodedText)
        {
            var actual = new ProcessExcelFormulaToLink().GetLinkFromFormula(formula,cellText);
            Assert.AreEqual(actual, encodedText);
        }
    }
}
