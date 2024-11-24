using Volo.Abp;

namespace Pusula.Training.HealthCare.Exceptions;

public class DateNotInPastException() : BusinessException("DateNotInPast", "Date cannot be in past");