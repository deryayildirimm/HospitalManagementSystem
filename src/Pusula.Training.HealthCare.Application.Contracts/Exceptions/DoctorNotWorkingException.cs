using Volo.Abp;

namespace Pusula.Training.HealthCare.Exceptions;

public class DoctorNotWorkingException()
    : BusinessException("DoctorNotWorking", "Selected doctor does not work on exact day.");