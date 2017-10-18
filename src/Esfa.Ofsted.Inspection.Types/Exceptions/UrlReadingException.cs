using System;

namespace Esfa.Ofsted.Inspection.Types.Exceptions
{
    /// <summary>
    /// Exception thrown when a string containing a link errors when retrieving details
    /// </summary>
    public class UrlReadingException: Exception
    {
        /// <summary>
        /// defauilt UrlReadingException with no message
        /// </summary>public class NoLinksInPageException: Exception
        public UrlReadingException()
        {
        }

        /// <summary>
        /// UrlReadingException with more detailed message
        /// </summary>
        /// <param name="message"></param>
        public UrlReadingException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// UrlReadingException with more detailed message, and passing through inner exception
        /// </summary>
        /// <param name="message">Message about details from outer exception</param>
        /// <param name="inner">Uri Reading Exception</param>
        public UrlReadingException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
