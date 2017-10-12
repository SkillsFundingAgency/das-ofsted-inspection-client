using System;

namespace Esfa.Ofsted.Inspection.Client.Exceptions
{
    public class UnmatchedEffectivenessException : Exception
    {       
        public UnmatchedEffectivenessException(string message)
            : base(message)
        {
        }
    }
}
