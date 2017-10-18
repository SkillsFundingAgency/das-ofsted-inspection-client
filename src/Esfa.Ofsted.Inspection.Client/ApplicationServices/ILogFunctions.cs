using System;

namespace Esfa.Ofsted.Inspection.Client.ApplicationServices
{
    /// <summary>
    /// Logging Functions that can be overridden
    /// </summary>
    public interface ILogFunctions
    {
        /// <summary>
        /// Method called for debug status logging
        /// </summary>
        Action<string> Debug { get; set; }
        /// <summary>
        /// Method called for info status logging
        /// </summary>
        Action<string> Info { get; set; }
        /// <summary>
        /// Method called for Warning status with exception
        /// </summary>
        Action<string, Exception> Warn { get; set; }
        /// <summary>
        /// Method called for Error status with Exception
        /// </summary>
        Action<string, Exception> Error { get; set; }
    }
}