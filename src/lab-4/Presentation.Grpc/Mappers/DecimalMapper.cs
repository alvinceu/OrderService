using System.Globalization;

namespace Presentation.Grpc.Mappers;

internal static class DecimalMapper
{
    public static decimal ToDecimal(this string value)
    {
        return decimal.Parse(value, NumberStyles.Number, CultureInfo.InvariantCulture);
    }

    public static string ToString(this decimal value)
    {
        return value.ToString("F99", CultureInfo.InvariantCulture).TrimEnd('0').TrimEnd('.');
    }
}