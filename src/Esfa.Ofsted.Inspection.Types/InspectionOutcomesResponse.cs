using System.Collections.Generic;

namespace Esfa.Ofsted.Inspection.Types
{
    /// <summary>
    /// Details returned from GetOfstedInspections(), consisting of a list of Inspection Outcomes, a list of errors, and a status code
    /// </summary>
    public class InspectionOutcomesResponse
    {
        /// <summary>
        /// List of Ofsted Inspection details that have been successfully retrieved
        /// </summary>
        public List<InspectionOutcome> InspectionOutcomes { get; set; }
        /// <summary>
        /// List of error messages (with line number, where available) about processed details
        /// </summary>
        public List<InspectionError> InspectionOutcomeErrors { get; set; }
        /// <summary>
        /// Status Code of returned details (success, processed with 1 or more errors, not processed)
        /// </summary>
        public InspectionsStatusCode StatusCode { get; set; }
    }
}