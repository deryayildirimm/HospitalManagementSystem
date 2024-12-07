using System.Collections.Generic;
using Volo.Abp;

namespace Pusula.Training.HealthCare.GlobalExceptions;

public class HealthcareGlobalException : IHealthcareGlobalException
{
    private const string DEFAULT_ERROR_MESSAGE = "An error has occurred.";
    private const string DEFAULT_ERROR_CODE = "500";

    public static void Throw(string message) => ThrowIf(message, true);
    public static void ThrowIf(string message) => ThrowIf(message, true);
    public static void ThrowIf(string message, bool condition) => ThrowIf(message, default, condition);

    public static void ThrowIf(KeyValuePair<string, string> pair, bool condition) =>
        ThrowIf(pair.Value, pair.Key, condition);

    public static void ThrowIf(string message, string code, bool condition)
    {
        if (condition)
            ThrowException(message, code);
    }

    private static void ThrowException(string message, string code) =>
        throw new UserFriendlyException(message ?? DEFAULT_ERROR_MESSAGE, code ?? DEFAULT_ERROR_CODE);
}