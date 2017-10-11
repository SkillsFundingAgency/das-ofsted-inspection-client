using System;
using System.Collections.Generic;

namespace Sfa.Das.Ofsted.Inspection.Types
{
    public class InspectionsDetail
    {
        public List<Inspection> Inspections { get; set; }
        public List<InspectionError> ErrorSet { get; set; }
        public InspectionsStatusCode StatusCode { get; set; }

    }
}