using System;

namespace Esfa.Ofsted.Inspection.Types.Exceptions
{
    /// <summary>
    /// Exception when a valid link cannot be constructed from site and path strings
    /// </summary>
    public class InvalidLinkException: Exception
    {
        /// <summary>
        /// defauilt InvalidLinkException with no message
        /// </summary>public class NoLinksInPageException: Exception
        public InvalidLinkException()
        {
        }

        /// <summary>
        /// InvalidLinkException with more detailed message
        /// </summary>
        /// <param name="message"></param>
        public InvalidLinkException(string message)
            : base(message)
        {
        }
    }
}
