using System.Configuration;
using Esfa.Ofsted.Inspection.Client.Services.Interfaces;

namespace Esfa.Ofsted.Inspection.Client.Services
{
    internal class ConfigurationSettings : IConfigurationSettings
    {
   
        public string LinkText => ConfigurationManager.AppSettings["LinkText"];
        public string InspectionSiteUrl => ConfigurationManager.AppSettings["InspectionSiteUrl"];
        public string WorksheetName => ConfigurationManager.AppSettings["WorksheetName"];
        public string WebLinkHeading => ConfigurationManager.AppSettings["WebLinkHeading"];
        public string UkPrnHeading => ConfigurationManager.AppSettings["UkPrnHeading"];
        public string DatePublishedHeading => ConfigurationManager.AppSettings["DatePublishedHeading"];
        public string OverallEffectivenessHeading => ConfigurationManager.AppSettings["OverallEffectivenessHeading"];

    }
}
