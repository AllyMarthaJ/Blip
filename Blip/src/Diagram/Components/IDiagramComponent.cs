namespace Blip.Diagram.Components;

public interface IDiagramComponent {
    // Components must own their width and height
    // and must render accordingly.

    public int MaxWidth { get; set; }
    public int MaxHeight { get; set; }

    public IEnumerable<IDiagramComponent> Children { get; }

    public StringMap AsStringMap();
}