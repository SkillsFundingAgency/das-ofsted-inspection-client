using Esfa.Ofsted.Inspection.Types;

namespace Esfa.Ofsted.Inspection.Client
{
    /// <summary>
    /// Get Ofsted Inspection Details
    /// </summary>
    public interface IOfstedInspectionsClient
    {
        /// <summary>
        /// Get All InspectionOutcomes
        /// </summary>
        /// <returns></returns>
        InspectionOutcomesResponse GetOfstedInspectionOutcomes();
    }
}
