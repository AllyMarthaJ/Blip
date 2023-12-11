using System.Text;

namespace Blip.Format;

public static class StringExtensions {
    public static string Justify(this string str, int len, Alignment alignment, char padWith = ' ') {
        int spacesRem = len - str.Length;

        // Don't align to a non-fixed size.
        if (str.Length > len) {
            return str;
        }

        return alignment switch {
            Alignment.LEFT => str + new string(padWith, spacesRem),
            Alignment.CENTER => new string(padWith, spacesRem / 2) + str +
                                new string(padWith, spacesRem - spacesRem / 2),
            Alignment.RIGHT => new string(padWith, spacesRem) + str,
            Alignment.JUSTIFY => alignJustify(new StringBuilder(str), len, padWith).ToString(),
            _ => throw new ArgumentOutOfRangeException(nameof(alignment), alignment, null)
        };
    }

    private static StringBuilder alignJustify(StringBuilder sb, int len, char padWith = ' ') {
        // TODO: Account for proper word whitespace.
        string[] words = sb.ToString().Split(" ");

        if (words.Length < 1) {
            sb.Append(new string(padWith, len - sb.Length));
            return sb;
        }

        sb.Clear();
        if (words.Length > 1) {
            int[] spaces = SharedHelpers.GetJustifySpaces(
                len,
                words.Sum((w) => w.Length),
                words.Length
            );

            for (var i = 0; i < words.Length - 1; i++) {
                sb.Append(words[i] + new string(padWith, spaces[i]));
            }
        }

        sb.Append(words[^1]);
        sb.Append(new string(padWith, len - sb.Length));

        return sb;
    }
}