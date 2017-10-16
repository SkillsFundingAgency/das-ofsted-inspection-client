namespace Esfa.Ofsted.Inspection.Client.Services.Interfaces
{
    internal interface IConfigurationSettings
    {
        string LinkText { get; }
        string InspectionSiteUrl { get; }
        string WorksheetName { get; }
    }
}