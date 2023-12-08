using System.Text;

namespace Blip.Formatters; 

public static class StringExtensions {
    public static string Justify(this string str, int len, Alignment alignment) {
        int spacesRem = len - str.Length;
        
        return alignment switch {
            Alignment.LEFT => str + new string(' ', spacesRem),
            Alignment.CENTER => new string(' ', spacesRem / 2) + str + new string(' ', spacesRem - spacesRem / 2),
            Alignment.RIGHT => new string(' ', spacesRem) + str,
            Alignment.JUSTIFY => alignJustify(new StringBuilder(str), len).ToString(),
            _ => throw new ArgumentOutOfRangeException(nameof(alignment), alignment, null)
        };
    }

    private static StringBuilder alignJustify(StringBuilder sb, int len) {
        // TODO: Account for proper word whitespace.
        string[] words = sb.ToString().Split(" ");

        if (words.Length < 1) {
            sb.Append(new string(' ', len - sb.Length));
            return sb;
        }

        int totalSpaces = words.Length - 1;
        int spaceRemaining = len - words
            .Select((w) => w.Length)
            .Aggregate(0, (tot, cur) => tot + cur);
        var usedSpace = 0;

        sb.Clear();
        if (totalSpaces > 0) {
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
        
            for (var i = 0; i < totalSpaces; i++) {
                sb.Append(words[i] + new string(' ', spaces[i]));
            }
        }

        sb.Append(words[^1]);
        sb.Append(new string(' ', len - sb.Length));

        return sb;
    }
}