using Esfa.Ofsted.Inspection.Client.Services.Interfaces;

namespace Esfa.Ofsted.Inspection.Client.Services
{
    internal class ConfigurationSettings : IConfigurationSettings
    {
        public string LinkText => ConfigurationResources.LinkText;
        public string InspectionSiteUrl => ConfigurationResources.InspectionSiteUrl;
        public string WorksheetName => ConfigurationResources.WorksheetName;

        public string WebLinkHeading => "Web link";
        public string UkPrnHeading => "Provider UKPRN";
        public string DatePublishedHeading => "Date published";
        public string OverallEffectivenessHeading => "Overall effectiveness";

    }
}
