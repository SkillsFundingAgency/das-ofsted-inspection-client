using Esfa.Ofsted.Inspection.Types;

namespace Esfa.Ofsted.Inspection.Client.Services.Interfaces
{
    internal interface IOverallEffectivenessProcessor
    {
        OverallEffectiveness GetOverallEffectiveness(string overallEffectivenessString);
    }
}