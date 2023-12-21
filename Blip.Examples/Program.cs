using Blip.Diagram.Components;
using ExtendedXmlSerializer;
using ExtendedXmlSerializer.Configuration;

namespace Blip.Examples;

class Program {
    static void Main(string[] args) {
        IExtendedXmlSerializer serializer = new
                ConfigurationContainer()
            .UseAutoFormatting()
            .UseOptimizedNamespaces()
            .EnableReferences()
            .EnableImplicitTyping(typeof(IDiagramComponent), typeof(Box), typeof(Flow), typeof(Frame),
                typeof(Text),
                typeof(Tree), typeof(IOComponent))
            .Create();
        
        var document = serializer.Deserialize<IDiagramComponent>(Console.In);
        
        Console.WriteLine(document.AsStringMap().ToString());
    }
}