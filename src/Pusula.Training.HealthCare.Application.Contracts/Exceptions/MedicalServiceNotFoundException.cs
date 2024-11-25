using Volo.Abp;

namespace Pusula.Training.HealthCare.Exceptions;

public class MedicalServiceNotFoundException()
    : BusinessException("MedicalServiceNotFound", "MedicalService Not Found.");