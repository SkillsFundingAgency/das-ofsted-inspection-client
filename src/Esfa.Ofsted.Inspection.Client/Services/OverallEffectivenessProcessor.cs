using Sfa.Das.Ofsted.Inspection.Types;

namespace Esfa.Ofsted.Inspection.Client.Services
{
    public class OverallEffectivenessProcessor: IOverallEffectivenessProcessor
    {
        public OverallEffectiveness GetOverallEffectiveness(string overallEffectivenessString)
        {
            switch (overallEffectivenessString)
            {
                case "1":
                    return OverallEffectiveness.Outstanding;
                case "2":
                    return OverallEffectiveness.Good;
                case "3":
                    return OverallEffectiveness.RequiresImprovement;
                case "4":
                    return OverallEffectiveness.Inadequate;
                case "9":
                    return OverallEffectiveness.RemainedGoodAtAShortInspectionThatDidNotConvert;
                default:
                    return OverallEffectiveness.NotJudged;
            }        
        }
    }

   
}
