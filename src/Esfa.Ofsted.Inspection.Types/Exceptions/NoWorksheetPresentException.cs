using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Esfa.Ofsted.Inspection.Types.Exceptions
{
    public class NoWorksheetPresentException: Exception
    {
        /// <summary>
        /// defauilt NoWorksheetPresentException with no message
        /// </summary>public class NoWorksheetPresentException: Exception
        public NoWorksheetPresentException()
        {
        }

        /// <summary>
        /// NoWorksheetPresentException with more detailed message
        /// </summary>
        /// <param name="message"></param>
        public NoWorksheetPresentException(string message)
            : base(message)
        {
        }
    }
}
