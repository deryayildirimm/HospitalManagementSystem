using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Pusula.Training.HealthCare.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<T> WhereIf<T>(
        this IQueryable<T> source,
        bool condition,
        Expression<Func<T, bool>> predicate)
    {
        return condition ? source.Where(predicate) : source;
    }
    
    public static string SafeGet(this string? value, string defaultValue = "Unknown") =>
        string.IsNullOrWhiteSpace(value) ? defaultValue : value;

    public static T SafeGet<T>(this T? value, T defaultValue) where T : struct =>
        value ?? defaultValue;
    
    public static IEnumerable<T> SafeWhere<T>(this IEnumerable<T>? source, Func<T, bool> predicate)
    {
        return source?.Where(predicate) ?? Enumerable.Empty<T>();
    }

    public static decimal SafeSum<T>(this IEnumerable<T>? source, Func<T, decimal?> selector)
    {
        return source?.Sum(selector) ?? 0;
    }
    

}