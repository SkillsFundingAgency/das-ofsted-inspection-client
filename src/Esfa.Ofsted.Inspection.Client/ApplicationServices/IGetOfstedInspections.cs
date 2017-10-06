using System.Collections.Generic;

namespace Esfa.Ofsted.Inspection.Client.ApplicationServices
{
    public interface IGetOfstedInspections
    {
        List<Sfa.Das.Ofsted.Inspection.Types.Inspection> GetAll();
    }
}
