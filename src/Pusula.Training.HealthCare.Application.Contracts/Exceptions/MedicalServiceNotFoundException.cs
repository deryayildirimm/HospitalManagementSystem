using Volo.Abp;

namespace Pusula.Training.HealthCare.Exceptions;

public class MedicalServiceNotFoundException()
    : BusinessException("DoctorNotWorking", "MedicalService Not Found.");