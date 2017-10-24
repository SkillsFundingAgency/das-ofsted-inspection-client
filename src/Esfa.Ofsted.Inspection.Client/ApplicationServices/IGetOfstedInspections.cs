using System.Collections.Generic;
using Esfa.Ofsted.Inspection.Types;

namespace Esfa.Ofsted.Inspection.Client.ApplicationServices
{
    /// <summary>
    /// Get Ofsted Inspection Details
    /// </summary>
    public interface IGetOfstedInspections
    {
        /// <summary>
        /// Get All InspectionOutcomes
        /// </summary>
        /// <returns></returns>
        InspectionOutcomesResponse GetAll();
    }
}
