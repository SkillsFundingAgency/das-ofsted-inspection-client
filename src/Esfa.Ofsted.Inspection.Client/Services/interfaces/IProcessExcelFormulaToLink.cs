namespace Esfa.Ofsted.Inspection.Client.Services.Interfaces
{
    internal interface IProcessExcelFormulaToLink
    {
        string GetLinkFromFormula(string hyperlinkFormula, string cellText);

    }
}
