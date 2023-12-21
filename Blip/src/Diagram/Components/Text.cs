using Blip.Format;
using ExtendedXmlSerializer.ContentModel.Content;

namespace Blip.Diagram.Components;

public class Text : IDiagramComponent {
    public Text() {
        this.Content = "";
    }

    public Text(Alignment alignment, string text) {
        this.Content = text;
        this.Alignment = alignment;
    }

    [Verbatim]
    public string Content { get; set; }
    public Alignment Alignment { get; set; } = Alignment.LEFT;
    public int MaxWidth { get; set; }
    public int MaxHeight { get; set; }

    public IEnumerable<IDiagramComponent> Children => new List<IDiagramComponent>().AsReadOnly();

    public StringMap AsStringMap() {
        var fmt = new WordSplitFormatter(this.Alignment);

        string[] lines = SharedHelpers.SPLIT_LINE_REGEX.Split(this.Content);
        int maxLineLength = lines.Select(line => line.Length).Max();

        int width = this.MaxWidth > 0 ? Math.Min(maxLineLength, this.MaxWidth) : maxLineLength;

        int height = this.MaxHeight > 0
            ? Math.Min(fmt.MeasureHeight(this.Content, width), this.MaxHeight)
            : fmt.MeasureHeight(this.Content, width);

        return new StringMap(width, height)
            .DrawString(this.Content, fmt, 0, 0, width, height);
    }
}