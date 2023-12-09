namespace Blip.Format;

public class TruncationFormatter(Alignment alignment) : IStringFormatter {
    public char[] FormatString(string str, int width, int height) {
        string[] formattedLines = this.getFormattedLines(str, width).ToArray();

        int maxLines = Math.Min(formattedLines.Length, height);

        return formattedLines[..maxLines].SelectMany(c => c).ToArray();
    }

    public int MeasureHeight(string str, int width) {
        return this.getFormattedLines(str, width).Count();
    }

    private IEnumerable<string> getFormattedLines(string str, int width) {
        string[] lines = SharedHelpers.SPLIT_LINE_REGEX.Split(str);
        return lines
            .SelectMany(line => this.formatLine(line, width));
    }

    private string[] formatLine(string str, int width) {
        // Ensure uniform length. Truncate with ellipses if
        // too short, pad with space if too long.
        if (str.Length > width) {
            return new[] { str[..(width - 3)] + "..." };
        }

        return new[] { str.Justify(width, alignment) };
    }
}