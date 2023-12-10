using Blip.Transform;

namespace Blip.Diagram.Components;

/// <summary>
/// Encapsulate a StringMap directly as a component.
/// Does not allow modification of the underlying StringMap directly.
/// Will crop the StringMap if the MaxWidth/MaxHeight are smaller
/// than the map itself.
/// </summary>
/// <param name="sm">The StringMap to encapsulate</param>
public class Raw(StringMap sm) : IDiagramComponent {
    public int MaxWidth { get; set; }
    public int MaxHeight { get; set; }
    public IEnumerable<IDiagramComponent> Children => new List<IDiagramComponent>().AsReadOnly();

    public StringMap AsStringMap() {
        bool shouldCopy =
            (this.MaxWidth != 0 && this.MaxWidth < sm.Width) ||
            (this.MaxHeight != 0 && this.MaxHeight < sm.Height);

        if (!shouldCopy) {
            return sm;
        }

        var width = this.MaxWidth == 0 ? sm.Width : this.MaxWidth;
        var height = this.MaxHeight == 0 ? sm.Height : this.MaxHeight;

        return new StringMap(width, height)
            .DrawStringMap(sm, new OriginOnlyTransform(), 0, 0, width, height);
    }
}