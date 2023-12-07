using System.Text;

namespace Blip.Formatters;

public class WordSplitFormatter(Alignment alignment) : IStringFormatter {
    public char[] FormatString(string str, int width, int height) {
        string[] lines = SharedHelpers.SPLIT_LINE_REGEX.Split(str);
        string[] formattedLines = lines.SelectMany(line => this.formatLine(line, width)).ToArray();

        int maxLines = Math.Min(formattedLines.Length, height);

        // TODO: Add ellipses when truncating lines.
        return formattedLines[..maxLines].SelectMany(c => c).ToArray();
    }

    private string spaceLine(StringBuilder sb, int width) {
        int spaceRemaining = width - sb.Length;

        return alignment switch {
            Alignment.LEFT => sb + new string(' ', spaceRemaining),
            Alignment.CENTER => new string(' ', spaceRemaining / 2) + sb +
                                new string(' ', spaceRemaining - spaceRemaining / 2),
            Alignment.RIGHT => new string(' ', spaceRemaining),
            _ => throw new ArgumentOutOfRangeException(nameof(alignment), alignment, null)
        };
    }

    private string[] formatLine(string str, int width) {
        if (str.Length <= width) return new[] { str };

        // TODO: Handle all whitespace.
        StringBuilder sb = new();
        var words = new Queue<string>(str.Split(" "));
        List<string> lines = new();

        while (words.Count > 0) {
            string word = words.Dequeue();

            // If the word exceeds the width, we clearly can't fit it on the
            // current line. Pop the line off, with padding.
            if (word.Length > width) {
                if (sb.Length > 0) {
                    lines.Add(this.spaceLine(sb, width));
                    sb = new StringBuilder();
                }

                lines.Add(word[..(width - 3)] + "...");
                continue;
            }

            if (sb.Length == 0) {
                sb.Append(word);
            }
            else {
                // Something's already in the builder, and so 
                // we need enough room for at least one space, plus the word.
                if (word.Length + sb.Length + 1 > width) {
                    lines.Add(this.spaceLine(sb, width));
                    sb = new StringBuilder(word);
                }
                else {
                    sb.Append(" " + word);
                }
            }
        }

        // Finished adding all the words.
        lines.Add(this.spaceLine(sb, width));

        return lines.ToArray();
    }
}