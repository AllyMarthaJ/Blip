using System.Text.RegularExpressions;

namespace Blip;

public static partial class SharedHelpers {
    public static readonly Regex SPLIT_LINE_REGEX = SplitLine();

    [GeneratedRegex(@"\r?\n", RegexOptions.Compiled)]
    private static partial Regex SplitLine();
}