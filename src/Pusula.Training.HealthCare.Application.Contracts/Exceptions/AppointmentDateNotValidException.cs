using Volo.Abp;

namespace Pusula.Training.HealthCare.Exceptions;

public class AppointmentDateNotValidException()
    : BusinessException("AppointmentAlreadyTaken", "Not valid date.");