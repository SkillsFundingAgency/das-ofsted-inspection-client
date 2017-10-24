using System;

namespace Esfa.Ofsted.Inspection.Client.ApplicationServices
{
  /// <summary>
  /// Logging Functions that can be overridden
  /// </summary>
  public class LogFunctions : ILogFunctions
    {
        /// <summary>
        /// Action called for debug status logging
        /// </summary>
        public Action<string> Debug { get; set; } = x => Console.WriteLine(x);
        /// <summary>
        /// Action called for info status logging
        /// </summary>
        public Action<string> Info { get; set; } = x => Console.WriteLine(x);
        /// <summary>
        /// Action called for warning status logging
        /// </summary>
        public Action<string> Warn { get; set; } = x => Console.WriteLine(x);
        /// <summary>
        /// Action called for Error status with Exception
        /// </summary>
        public Action<string, Exception> Error { get; set; } = (x, y) => Console.WriteLine(x);
    }
}
