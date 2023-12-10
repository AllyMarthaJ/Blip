using Blip.Format;

namespace Blip.Diagram.Components;

public class Frame : IDiagramComponent {
    public Alignment HorizontalAlignment { get; set; } = Alignment.CENTER;
    public Alignment VerticalAlignment { get; set; } = Alignment.CENTER;
    public int MaxWidth { get; set; }
    public int MaxHeight { get; set; }

    public IEnumerable<IDiagramComponent> Children { get; set; }

    public StringMap AsStringMap() {
        if (this.Children.Count() > 1) {
            throw new ArgumentException("Frames may only have one child.");
        }

        StringMap childMap = this.Children.First().AsStringMap();
        if (
            (this.MaxWidth == 0 || this.MaxWidth == childMap.Width) &&
            (this.MaxHeight == 0 || this.MaxHeight == childMap.Height)
        ) {
            return childMap;
        }

        int width = this.MaxWidth > 0 ? this.MaxWidth : childMap.Width;
        int height = this.MaxHeight > 0 ? this.MaxHeight : childMap.Height;

        int left = this.HorizontalAlignment switch {
            Alignment.LEFT => 0,
            Alignment.CENTER => (width - childMap.Width) / 2,
            Alignment.RIGHT => width - childMap.Width,
            Alignment.JUSTIFY => throw new NotSupportedException(),
            _ => throw new ArgumentOutOfRangeException()
        };

        int top = this.VerticalAlignment switch {
            Alignment.LEFT => 0,
            Alignment.CENTER => (height - childMap.Height) / 2,
            Alignment.RIGHT => height - childMap.Height,
            Alignment.JUSTIFY => throw new NotSupportedException(),
            _ => throw new ArgumentOutOfRangeException()
        };

        return new StringMap(width, height)
            .DrawStringMap(childMap, left, top);
    }
}