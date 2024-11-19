using Volo.Abp;

namespace Pusula.Training.HealthCare.Exceptions;

public class AppointmentAlreadyTakenException()
    : BusinessException("AppointmentAlreadyTaken", "Selected appointment already taken.");