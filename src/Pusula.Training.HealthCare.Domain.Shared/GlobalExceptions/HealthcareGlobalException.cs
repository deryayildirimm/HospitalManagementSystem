using System.Collections.Generic;
using Volo.Abp;

namespace Pusula.Training.HealthCare.GlobalExceptions;

public class HealthCareGlobalException :IHealthCareGlobalException
{
    
    public static void Throw(string message) => ThrowIf(message, true);

    public static void ThrowIf(string message) => ThrowIf(message, true);

    public static void ThrowIf(string message, bool condition) => ThrowIf(message, default, true);
    
    public static void ThrowIf(KeyValuePair<string, string> pair, bool condition) =>
        ThrowIf(pair.Value, pair.Key, condition);

    public static void ThrowIf(string message, string code, bool condition)
    {
        if (condition)
            ThrowException(message, code);
    }
    
    public static void ThrowException(string message, string code) => new UserFriendlyException(message ?? HealthCareDomainErrorCodes.HealthcareError_MESSAGE , code ?? HealthCareDomainErrorCodes.HealthcareError_CODE);
    
    
}