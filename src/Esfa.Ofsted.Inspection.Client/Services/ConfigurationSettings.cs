using Esfa.Ofsted.Inspection.Client.Services.Interfaces;

namespace Esfa.Ofsted.Inspection.Client.Services
{
    internal class ConfigurationSettings : IConfigurationSettings
    {
        public string LinkText => "Management information";
        public string InspectionSiteUrl => "https://www.gov.uk/government/statistical-data-sets/monthly-management-information-ofsteds-further-education-and-skills-inspections-outcomes-from-december-2015";
        public string WorksheetName => "D1 In-year inspection data";
    }
}
