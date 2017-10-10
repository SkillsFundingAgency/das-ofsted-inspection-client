using System;
using Esfa.Ofsted.Inspection.Client.Services.interfaces;

namespace Esfa.Ofsted.Inspection.Client.Services
{
    public class ProcessExcelFormulaToLink : IProcessExcelFormulaToLink
    {
        public string GetLinkFromFormula(string hyperlinkFormula)
        {
            var splitDetails = hyperlinkFormula.Split('"');
            if (splitDetails.Length < 2)
                return string.Empty;

            var url = splitDetails[1].Trim();

            Uri uriResult;
            var result = Uri.TryCreate(url, UriKind.Absolute, out uriResult)
                          && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

            return result == false ? string.Empty : url;
        }
    }
}