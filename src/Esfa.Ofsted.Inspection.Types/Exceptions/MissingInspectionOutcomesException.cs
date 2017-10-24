using System;

namespace Esfa.Ofsted.Inspection.Types.Exceptions
{
    /// <summary>
    /// Exception for a page where there are no usable details
    /// </summary>
    public class MissingInspectionOutcomesException : Exception
    {
        /// <summary>
        /// defauilt MissingInspectionOutcomesException with no message
        /// </summary>public class MissingInspectionOutcomesException: Exception
        public MissingInspectionOutcomesException()
        {
        }

        /// <summary>
        /// MissingInspectionOutcomesException with more detailed message.  If individual line errors are present, then these can be found in the Exception.Data Dictionary
        /// </summary>
        /// <param name="message"></param>
        public MissingInspectionOutcomesException(string message)
            : base(message)
        {
        }
    }
}
