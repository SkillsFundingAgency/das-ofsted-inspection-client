using Esfa.Ofsted.Inspection.Client.Services;
using Esfa.Ofsted.Inspection.Types;
using NUnit.Framework;

namespace Esfa.Ofsted.Inspection.UnitTests
{
    [TestFixture]
    public class OverallEffectivenessProcessorTests
    {
        [TestCase("1", OverallEffectiveness.Outstanding)]
        [TestCase("2", OverallEffectiveness.Good)]
        [TestCase("3", OverallEffectiveness.RequiresImprovement)]
        [TestCase("4", OverallEffectiveness.Inadequate)]
        [TestCase("9", OverallEffectiveness.RemainedGoodAtAShortInspectionThatDidNotConvert)]
        [TestCase("-", OverallEffectiveness.NotJudged)]
        [TestCase(null, OverallEffectiveness.NotJudged)]
       
        public void ShouldReturnStringModifiedForUrlUsage(string inputText, OverallEffectiveness expectedEffectiveness)
        {
            var actual = new OverallEffectivenessProcessor().GetOverallEffectiveness(inputText);
           Assert.AreEqual(actual, expectedEffectiveness);
        }
        
        [TestCase("")]
        [TestCase("sdv")]
        [TestCase("22")]
        [TestCase("--")]
        [TestCase("01")]
        public void ShouldReturnExceptionForOddEffectivenessValues(string inputText)
        {

            var actual = new OverallEffectivenessProcessor().GetOverallEffectiveness(inputText);
            Assert.IsNull(actual,"An invalid value should be returning null");
        }
    }
}
