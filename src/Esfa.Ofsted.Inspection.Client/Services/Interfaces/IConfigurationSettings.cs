namespace Esfa.Ofsted.Inspection.Client.Services.Interfaces
{
    internal interface IConfigurationSettings
    {
        string LinkText { get; }
        string InspectionSiteUrl { get; }
        string WorksheetName { get; }
        string WebLinkHeading { get; }
        string UkPrnHeading { get; }
        string DatePublishedHeading { get; }
        string OverallEffectivenessHeading { get; }
    }
}