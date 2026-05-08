using System.Collections.Generic;
using System.Globalization;

namespace JADY.Core.Data;

public static class AppCultures
{
    public static List<CultureInfo> AvailableCultures { get; } = new()
    {
        new CultureInfo("cs-CZ"),
        new CultureInfo("en-US"),
        new CultureInfo("en-GB"),
        new CultureInfo("de-DE"),
        new CultureInfo("fr-FR"),
    };
}