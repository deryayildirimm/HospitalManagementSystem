using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pusula.Training.HealthCare.BloodTests
{
    public enum BloodTestStatus
    {
        Requested = 1,    
        InProgress = 2,   
        PendingResults = 3,
        Completed = 4,    
        Cancelled = 5     
    }
}
