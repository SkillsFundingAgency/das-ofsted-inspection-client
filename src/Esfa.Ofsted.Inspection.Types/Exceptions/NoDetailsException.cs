using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Esfa.Ofsted.Inspection.Types.Exceptions
{
    /// <summary>
    /// Exception for a page where there are no usable details
    /// </summary>
    public class NoDetailsException: Exception
    {
        /// <summary>
        /// defauilt NoDetailsException with no message
        /// </summary>public class NoDetailsException: Exception
        public NoDetailsException()
        { }

        /// <summary>
        /// NoDetailsException with more detailed message
        /// </summary>
        /// <param name="message"></param>
        public NoDetailsException(string message)
            : base(message)
        { }
    }
}
