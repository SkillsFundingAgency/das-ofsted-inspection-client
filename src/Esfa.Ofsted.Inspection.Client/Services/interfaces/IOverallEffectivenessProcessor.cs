using Sfa.Das.Ofsted.Inspection.Types;

namespace Esfa.Ofsted.Inspection.Client.Services.interfaces
{
    public interface IOverallEffectivenessProcessor
    {
        OverallEffectiveness GetOverallEffectiveness(string overallEffectivenessString);
    }
}