using System.Collections.ObjectModel;
using Blip.Diagram.Styles;
using Blip.Format;

namespace Blip.Diagram.Components;

public class Box(string? title, string message) : IDiagramComponent {
    public int MaxWidth { get; set; } = 50;
    public int MaxHeight { get; set; } = 30;

    public Padding TitlePadding { get; set; }
    public Padding MessagePadding { get; set; }

    public string? Title { get; set; } = title;
    public string Message { get; set; } = message;

    public Alignment TitleAlignment { get; set; } = Alignment.CENTER;
    public Alignment MessageAlignment { get; set; } = Alignment.JUSTIFY;

    public int BorderWidth { get; set; } = 1;
    public int BorderHeight { get; set; } = 1;

    public void FitToGoldenRatio() {
        // This is truly the most satisfying way to create a box.
    }

    public Box(string title, string message, Alignment titleAlign, Alignment messageAlign) : this(title, message) {
        this.TitleAlignment = titleAlign;
        this.MessageAlignment = messageAlign;
    }

    // Boxes do not support children.
    public IEnumerable<IDiagramComponent> Children => new List<IDiagramComponent>().AsReadOnly();

    public StringMap AsStringMap() {
        var titleFmt = new TruncationFormatter(this.TitleAlignment);
        var messageFmt = new WordSplitFormatter(this.MessageAlignment);

        var hasTitle = this.Title is not null;
        
        var borderWidth = this.BorderWidth;
        var borderHeight = this.BorderHeight;

        var width = this.MaxWidth;
        var height = this.MaxHeight;

        var titleLeft = this.TitlePadding.Left + borderWidth;
        var titleTop = this.TitlePadding.Top + borderHeight;
        var titleRight = this.TitlePadding.Right + borderWidth;

        var titleWidth = width - titleLeft - titleRight;

        var separatorLeft = borderWidth;
        var separatorTop = hasTitle ? titleTop + this.TitlePadding.Bottom + 1 : 0;
        var separatorWidth = width - 2 * borderWidth;

        var messageLeft = this.MessagePadding.Left + borderWidth;
        var messageTop = separatorTop + this.MessagePadding.Top + 1;
        var messageWidth = width - messageLeft - this.MessagePadding.Right - borderWidth;

        var actualMessageHeight = messageFmt.MeasureHeight(message, messageWidth);
        var maxMessageHeight = height - messageTop - this.MessagePadding.Bottom - borderHeight;

        var clampMessageHeight = Math.Min(maxMessageHeight, actualMessageHeight);

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
}