using System.Collections.Generic;

namespace Esfa.Ofsted.Inspection.Types
{
    public class InspectionsDetail
    {
        public List<OfstedInspection> Inspections { get; set; }
        public List<InspectionError> ErrorSet { get; set; }
        public InspectionsStatusCode StatusCode { get; set; }

    }
}