using System;

namespace Esfa.Ofsted.Inspection.Types.Exceptions
{
    /// <summary>
    /// Exception for a page that's found, but contains no links
    /// </summary>
    public class NoLinksInPageException : Exception
    {
        /// <summary>
        /// defauilt NoLinksInPageException with no message
        /// </summary>public class NoLinksInPageException: Exception
        public NoLinksInPageException()
        {}

        /// <summary>
        /// NoLinksInPageException with more detailed message
        /// </summary>
        /// <param name="message"></param>
        public NoLinksInPageException(string message)
            : base(message)
        {}
    }
}
