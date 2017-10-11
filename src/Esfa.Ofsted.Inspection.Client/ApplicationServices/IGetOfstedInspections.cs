using System.Collections.Generic;
using Sfa.Das.Ofsted.Inspection.Types;

namespace Esfa.Ofsted.Inspection.Client.ApplicationServices
{
    public interface IGetOfstedInspections
    {
        InspectionsDetail GetAll();
    }
}
