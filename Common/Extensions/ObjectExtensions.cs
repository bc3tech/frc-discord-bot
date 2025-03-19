namespace Common.Extensions;

using System.Diagnostics.CodeAnalysis;

public static class ObjectExtensions
{
    [return: NotNullIfNotNull(nameof(obj)), NotNullIfNotNull(nameof(other))]
    public static T? Or<T>(this T? obj, T? other) => obj ?? other;

    [return: NotNull]
    public static T OrException<T>(this T? obj, T? other) => Throws.IfNull(Or(obj, other));

    public static T? UnlessThen<T>(this T? value, Predicate<T?> condition, T? other) => condition(value) ? value : other;
}
