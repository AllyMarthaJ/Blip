using System.Text;

namespace Blip.Format;

public class WordSplitFormatter(Alignment alignment) : IStringFormatter {
    private IEnumerable<string> getFormattedLines(string str, int width) {
        string[] lines = SharedHelpers.SPLIT_LINE_REGEX.Split(str);
        return lines.SelectMany(line => this.formatLine(line, width));
    }
    
    public char[] FormatString(string str, int width, int height) {
        string[] formattedLines = this.getFormattedLines(str, width).ToArray();

        int maxLines = Math.Min(formattedLines.Length, height);

        // TODO: Add ellipses when truncating lines.
        return formattedLines[..maxLines].SelectMany(c => c).ToArray();
    }

    public int MeasureHeight(string str, int width) {
        return this.getFormattedLines(str, width).Count();
    }

    private string[] formatLine(string str, int width) {
        // TODO: Handle all whitespace.
        StringBuilder sb = new();
        Queue<string> words = new(str.Split(" "));
        List<string> lines = new();

        while (words.Count > 0) {
            string word = words.Dequeue();

            // If the word exceeds the width, we clearly can't fit it on the
            // current line. Pop the line off, with padding.
            if (word.Length > width) {
                if (sb.Length > 0) {
                    lines.Add(sb.ToString().Justify(width, alignment));
                    sb.Clear();
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
                    lines.Add(sb.ToString().Justify(width, alignment));
                    sb.Clear();
                    sb.Append(word);
                }
                else {
                    sb.Append(" " + word);
                }
            }
        }

        // Finished adding all the words.
        lines.Add(sb.ToString().Justify(width, alignment));

        return lines.ToArray();
    }
}