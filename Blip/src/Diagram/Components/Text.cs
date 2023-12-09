using Blip.Format;

namespace Blip.Diagram.Components;

public class Text(Alignment alignment, string text) : IDiagramComponent {
    public int MaxWidth { get; set; }
    public int MaxHeight { get; set; }

    public string Content { get; set; } = text;
    public Alignment Alignment { get; set; } = alignment;

    public IEnumerable<IDiagramComponent> Children => new List<IDiagramComponent>().AsReadOnly();

    public StringMap AsStringMap() {
        var fmt = new WordSplitFormatter(this.Alignment);

        var lines = SharedHelpers.SPLIT_LINE_REGEX.Split(this.Content);
        var maxLineLength = lines.Select(line => line.Length).Max();

        var width = this.MaxWidth > 0 ? Math.Min(maxLineLength, this.MaxWidth) : maxLineLength;

        var height = this.MaxHeight > 0
            ? Math.Min(fmt.MeasureHeight(text, width), this.MaxHeight)
            : fmt.MeasureHeight(text, width);

        return new StringMap(width, height)
            .DrawString(this.Content, fmt, 0, 0, width, height);
    }
}