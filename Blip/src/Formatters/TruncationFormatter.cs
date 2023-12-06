namespace Blip.Formatters;

public class TruncationFormatter(Alignment alignment) : IStringFormatter {
    public string[] FormatString(string str, int width, int height) {
        string[] lines = SharedHelpers.SPLIT_LINE_REGEX.Split(str);
        string[] formattedLines = lines.SelectMany((line) => this.formatLine(line, width)).ToArray();

        int maxLines = Math.Min(formattedLines.Length, height);

        return formattedLines[..maxLines];
    }

    private string[] formatLine(string str, int width) {
        // Ensure uniform length. Truncate with ellipses if
        // too short, pad with space if too long.
        if (str.Length > width) {
            return new[] { str[..(width - 3)] + "..." };
        }

        int strLength = width - str.Length;

        string padded = alignment switch {
            Alignment.LEFT => str + new string(' ', strLength),
            Alignment.CENTER => new string(' ', strLength / 2) + str + new string(' ', strLength - strLength / 2),
            Alignment.RIGHT => new string(' ', strLength) + str,
            _ => throw new ArgumentOutOfRangeException(nameof(alignment), alignment, null)
        };

        return new[] { padded };
    }
}