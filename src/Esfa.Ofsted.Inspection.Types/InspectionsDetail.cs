using System.Collections.Generic;

namespace Esfa.Ofsted.Inspection.Types
{
    /// <summary>
    /// Details returned from GetOfstedInspections(), consisting of a list of Inspections, a list of errors, and a status code
    /// </summary>
    public class InspectionsDetail
    {
        /// <summary>
        /// List of Ofsted Inspection details that have been successfully retrieved
        /// </summary>
        public List<OfstedInspection> Inspections { get; set; }
        /// <summary>
        /// List of error messages (with line number, where available) about processed details
        /// </summary>
        public List<InspectionError> ErrorSet { get; set; }
        /// <summary>
        /// Status Code of returned details (success, processed with 1 or more errors, not processed)
        /// </summary>
        public InspectionsStatusCode StatusCode { get; set; }
    }
}