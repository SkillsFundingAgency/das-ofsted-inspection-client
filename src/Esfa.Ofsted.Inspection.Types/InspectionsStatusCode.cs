namespace Esfa.Ofsted.Inspection.Types
{
    /// <summary>
    /// 
    /// </summary>
    public enum InspectionsStatusCode
    {
        /// <summary>
        /// Inspection details all processed without any issue
        /// </summary>
        Success,
        /// <summary>
        /// Inspection details processed, but one or more lines had errors
        /// </summary>
        ProcessedWithErrors,
        /// <summary>
        /// Inspection details failed at the file retrieval level, or no items were processed successfully (see associated error messages for more details)
        /// </summary>
        NotProcessed
    }
}
