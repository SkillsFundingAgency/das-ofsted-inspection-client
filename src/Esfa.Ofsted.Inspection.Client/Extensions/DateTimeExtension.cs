using System;
using System.Globalization;

namespace Esfa.Ofsted.Inspection.Client.Extensions
{
    internal static class DateTimeExtension
    {
        internal static string ToDdmmyyyyString(this DateTime? date)
        {
            return date?.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
        }
    }
}