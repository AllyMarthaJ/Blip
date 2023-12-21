using System.Text.RegularExpressions;
using Blip.Diagram.Components;
using ExtendedXmlSerializer;
using ExtendedXmlSerializer.Configuration;

namespace Blip;

public static partial class SharedHelpers {
    public static readonly Regex SPLIT_LINE_REGEX = SplitLine();

    [GeneratedRegex(@"\r?\n", RegexOptions.Compiled)]
    private static partial Regex SplitLine();

    public static int[] GetJustifySpaces(int totalWidth, int contentWidth, int contentCount) {
        int totalSpaces = contentCount - 1;
        int spaceRemaining = totalWidth - contentWidth;

        var usedSpace = 0;

        int[] spaces = Enumerable.Repeat(0, totalSpaces).ToArray();
        int curIdx = totalSpaces - 1;
        while (usedSpace < spaceRemaining) {
            spaces[curIdx] += 1;

            usedSpace += 1;
            curIdx -= 1;
            if (curIdx < 0) {
                curIdx = totalSpaces - 1;
            }
        }

        return spaces;
    }

    internal static IExtendedXmlSerializer DocumentSerializer = new
            ConfigurationContainer()
        .UseAutoFormatting()
        .UseOptimizedNamespaces()
        .EnableReferences()
        .EnableImplicitTyping(
            typeof(IDiagramComponent), 
            typeof(Box), typeof(Flow), 
            typeof(Frame),
            typeof(Text),
            typeof(Tree), 
            typeof(IOComponent))
        .Create();
}