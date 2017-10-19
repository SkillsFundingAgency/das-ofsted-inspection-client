using Esfa.Ofsted.Inspection.Client.Services.Interfaces;

namespace Esfa.Ofsted.Inspection.Client.Services
{
    internal class ProcessExcelFormulaToLink : IProcessExcelFormulaToLink
    {
        public string GetLinkFromFormula(string hyperlinkFormula, string text)
        {
            var urlFromHyperlink = ProcessFormulaToExtractUrl(hyperlinkFormula);

            if (urlFromHyperlink != string.Empty)
                return urlFromHyperlink;

          return !string.IsNullOrEmpty(text) ? text.Trim() : string.Empty;
        }

        private static string ProcessFormulaToExtractUrl(string hyperlinkFormula)
        {
            if (hyperlinkFormula == null)
                return string.Empty;

            var splitDetails = hyperlinkFormula.Split('"');

            return splitDetails.Length < 2 ? string.Empty : splitDetails[1].Trim();
        }
    }
}