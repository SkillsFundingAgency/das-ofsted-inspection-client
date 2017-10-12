using Esfa.Ofsted.Inspection.Client.Exceptions;
using Esfa.Ofsted.Inspection.Client.Services.Interfaces;
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
                case "-":
                    return OverallEffectiveness.NotJudged;
                case null:
                    return OverallEffectiveness.NotJudged;
                default:
                    throw new UnmatchedEffectivenessException($"Invalid Overall Effectiveness: [{overallEffectivenessString}]");
            }        
        }
    }

   
}
