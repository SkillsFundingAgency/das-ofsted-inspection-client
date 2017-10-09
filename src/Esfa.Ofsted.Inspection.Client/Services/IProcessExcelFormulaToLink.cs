using System.Globalization;
using OfficeOpenXml;

namespace Esfa.Ofsted.Inspection.Client.Services
{
    public interface IProcessExcelFormulaToLink
    {
        string GetLinkFromFormula(string hyperlinkFormula);

    }
}
