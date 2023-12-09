using Blip.Diagram.Styles;
using Blip.Format;

namespace Blip.Diagram.Components;

public class Box(string message, string? title = null) : IDiagramComponent {
    public Box(string title, string message, Alignment titleAlign, Alignment messageAlign) : this(title, message) {
        this.TitleAlignment = titleAlign;
        this.MessageAlignment = messageAlign;
    }

    public Padding TitlePadding { get; set; } = new() { Left = 1, Right = 1 };
    public Padding MessagePadding { get; set; } = new() { Top = 1, Bottom = 1, Left = 3, Right = 3 };

    public string? Title { get; set; } = title;
    public string Message { get; set; } = message;

    public Alignment TitleAlignment { get; set; } = Alignment.CENTER;
    public Alignment MessageAlignment { get; set; } = Alignment.JUSTIFY;

    public int BorderWidth { get; set; } = 1;
    public int BorderHeight { get; set; } = 1;
    public int MaxWidth { get; set; } = 40;
    public int MaxHeight { get; set; } = 30;

    // Boxes do not support children.
    public IEnumerable<IDiagramComponent> Children => new List<IDiagramComponent>().AsReadOnly();

    public StringMap AsStringMap() {
        var titleFmt = new TruncationFormatter(this.TitleAlignment);
        var messageFmt = new WordSplitFormatter(this.MessageAlignment);

        bool hasTitle = this.Title is not null;

        int borderWidth = this.BorderWidth;
        int borderHeight = this.BorderHeight;

        int width = this.MaxWidth;
        int height = this.MaxHeight;

        int titleLeft = this.TitlePadding.Left + borderWidth;
        int titleTop = this.TitlePadding.Top + borderHeight;
        int titleRight = this.TitlePadding.Right + borderWidth;

        int titleWidth = width - titleLeft - titleRight;

        int separatorLeft = borderWidth;
        int separatorTop = hasTitle ? titleTop + this.TitlePadding.Bottom + 1 : 0;
        int separatorWidth = width - 2 * borderWidth;

        int messageLeft = this.MessagePadding.Left + borderWidth;
        int messageTop =
            // If we have a title, take directly from the separator since we 
            // already computed the relevant position.
            // Otherwise take directly from the border; the message is the topmost thing.
            this.MessagePadding.Top + (hasTitle ? separatorTop + 1 : this.BorderHeight);
        int messageWidth = width - messageLeft - this.MessagePadding.Right - borderWidth;

        int actualMessageHeight = messageFmt.MeasureHeight(message, messageWidth);
        int maxMessageHeight = height - messageTop - this.MessagePadding.Bottom - borderHeight;

        int clampMessageHeight = Math.Min(maxMessageHeight, actualMessageHeight);

        height = messageTop + clampMessageHeight + this.MessagePadding.Bottom + borderHeight;

        StringMap sm = new StringMap(width, height)
            .FillRectangle('#', 0, 0, width, height)
            .FillRectangle(' ', borderWidth, borderHeight, width - 2 * borderWidth, height - 2 * borderHeight);
        if (hasTitle) {
            sm
                .DrawString(this.Title!, titleFmt, titleLeft, titleTop, titleWidth, 1)
                .FillRectangle('-', separatorLeft, separatorTop, separatorWidth, 1);
        }

        return sm
            .DrawString(this.Message, messageFmt, messageLeft, messageTop, messageWidth, clampMessageHeight);
    }

    public void FitToGoldenRatio() {
        // This is truly the most satisfying way to create a box.
    }
}