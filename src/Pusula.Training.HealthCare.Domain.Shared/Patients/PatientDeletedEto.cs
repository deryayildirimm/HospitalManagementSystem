using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Events.Distributed;

namespace Pusula.Training.HealthCare.Patients
{
    public class PatientDeletedEto : EtoBase
    {
        public int PatientNumber { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime DeletedAt { get; set; }
        public string? DeletedByUserName { get; set; }
    }
}
