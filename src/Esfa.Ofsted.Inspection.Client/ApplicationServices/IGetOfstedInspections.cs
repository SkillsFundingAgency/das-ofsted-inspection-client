using System.Collections.Generic;
using Esfa.Ofsted.Inspection.Types;

namespace Esfa.Ofsted.Inspection.Client.ApplicationServices
{
    /// <summary>
    /// Get All available Ofsted Inspection Details
    /// </summary>
    public interface IGetOfstedInspections
    {
        InspectionsDetail GetAll();
    }
}
