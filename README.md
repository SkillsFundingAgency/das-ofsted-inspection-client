# das-ofsted-inspection-client

|               |               |
| ------------- | ------------- |
|![crest](https://assets.publishing.service.gov.uk/static/images/govuk-crest-bb9e22aff7881b895c2ceb41d9340804451c474b883f09fe1b4026e76456f44b.png) ||
| Build | [![Build status](https://ci.appveyor.com/api/projects/status/j52iwixtxhqqnrbi?svg=true)](https://ci.appveyor.com/project/scottcowan/das-ofsted-inspection-client) |
| .Net Client |[![](https://img.shields.io/nuget/v/Esfa.Ofsted.Inspection.Client.svg)](https://www.nuget.org/packages/Esfa.Ofsted.Inspection.Client/)| 
| .Net Client Types |[![](https://img.shields.io/nuget/v/Esfa.Ofsted.Inspection.Types.svg)](https://www.nuget.org/packages/Esfa.Ofsted.Inspection.Types/)| 
| Source  | https://github.com/SkillsFundingAgency/das-ofsted-inspection-client  |

## A client for latest Ofsted Inspection Details

provides a client that returns a list of latest OFSTED report scores, publication dates, ukprns and website links for providers

returns the following details

``` csharp
public class InspectionsDetail
    {
        Inspections, // one or more inspection details  (Website,Ukprn,DatePublished,OverallEffectiveness)
        ErrorSet, // zero or more errors (Website,Ukprn,DatePublished,OverallEffectiveness, lineNumber, Message)
        StatusCode // 'Success', ' ProcessedWithErrors'
    }
```

