using System.Runtime.Remoting.Channels;
using Esfa.Ofsted.Inspection.Client.Services.Interfaces;

namespace Esfa.Ofsted.Inspection.Client.Services
{
    internal class ConfigurationSettings : IConfigurationSettings
    {
        public string LinkText => ConfigurationResources.LinkText;
        public string InspectionSiteUrl => ConfigurationResources.InspectionSiteUrl;
        public string WorksheetName => ConfigurationResources.WorksheetName;
    }
}
