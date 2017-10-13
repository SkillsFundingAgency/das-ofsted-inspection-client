﻿using Esfa.Ofsted.Inspection.Client.Services;
using NUnit.Framework;

namespace Esfa.Ofsted.Inspection.UnitTests
{
    [TestFixture]
    public class ProcessExcelFormulaToLinkTests
    {

        [TestCase("HYPERLINK(\"http://www.ofsted.gov.uk/inspection-reports/find-inspection-report/provider/ELS/54805  \",\"Ofsted Webpage\")", 
                              "http://www.ofsted.gov.uk/inspection-reports/find-inspection-report/provider/ELS/54805")]
        [TestCase("", "")]
        [TestCase("Some details without a hyperlink", "")]
        public void ShouldReturnStringModifiedForUrlUsage(string inputText, string encodedText)
        {
            var actual = new ProcessExcelFormulaToLink().GetLinkFromFormula(inputText);
            Assert.AreEqual(actual, encodedText);
        }
    }
}
