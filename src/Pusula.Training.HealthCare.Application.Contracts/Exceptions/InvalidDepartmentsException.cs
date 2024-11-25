using Volo.Abp;

namespace Pusula.Training.HealthCare.Exceptions;

public class InvalidDepartmentsException()
    : BusinessException("InvalidDepartments", "Invalid departments.");