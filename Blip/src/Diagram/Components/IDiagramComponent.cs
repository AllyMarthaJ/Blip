using ExtendedXmlSerializer;
using ExtendedXmlSerializer.Configuration;

namespace Blip.Diagram.Components;

public interface IDiagramComponent {
    // Components must own their width and height
    // and must render accordingly.

    public int MaxWidth { get; set; }
    public int MaxHeight { get; set; }

    public IEnumerable<IDiagramComponent> Children { get; }

    public StringMap AsStringMap();

    public string AsXml() {
        IExtendedXmlSerializer serializer = new
                ConfigurationContainer()
            .UseAutoFormatting()
            .UseOptimizedNamespaces()
            .EnableReferences()
            .EnableImplicitTyping(typeof(IDiagramComponent), typeof(Box), typeof(Flow), typeof(Frame), typeof(Text),
                typeof(Tree), typeof(IOComponent))
            .Create();

        return serializer.Serialize(this);
    }
}