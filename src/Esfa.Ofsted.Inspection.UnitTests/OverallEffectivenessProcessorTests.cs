using Esfa.Ofsted.Inspection.Client.Services;
using NUnit.Framework;
using Sfa.Das.Ofsted.Inspection.Types;

namespace Sfa.Das.Ofsted.Inspection.UnitTests
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
        [TestCase((string) null, OverallEffectiveness.NotJudged)]
        [TestCase("", OverallEffectiveness.NotJudged)]
        [TestCase("sdv", OverallEffectiveness.NotJudged)]

        public void ShouldReturnStringModifiedForUrlUsage(string inputText, OverallEffectiveness effectivness)
        {

            var actual = new OverallEffectivenessProcessor().GetOverallEffectiveness(inputText);
           Assert.AreEqual(actual, effectivness);
        }
    }
}
