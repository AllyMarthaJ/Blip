using ExtendedXmlSerializer;

namespace Blip.Diagram.Components;

public class IOComponent : IDiagramComponent {
    public int MaxWidth { get; set; }
    public int MaxHeight { get; set; }
    public IEnumerable<IDiagramComponent> Children { get; }

    public string Path { get; set; }

    public StringMap AsStringMap() {
        var content = File.ReadAllText(this.Path);
        
        var component = SharedHelpers.DocumentSerializer.Deserialize<IDiagramComponent>(content);
        
        return component.AsStringMap();
    }
}