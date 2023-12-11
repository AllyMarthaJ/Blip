using Blip.Diagram.Components;
using Blip.Format;

namespace Blip.Tests.Diagram.Components;

public class FlowTests {
    [Test]
    public void FlowBlah() {
        var children = new List<IDiagramComponent>();
        Flow flow = new() { MaxWidth = 11, Children = children, FlowAlignment = Alignment.JUSTIFY };

        for (int i = 0; i < 20; i++) {
            children.Add(new Text(Alignment.LEFT, i % 2 == 0 ? "hi" : "bye"));
        }

        for (int i = 20; i < 40; i += 3) {
            Console.WriteLine($"With width {i} ---------------");
            flow.MaxWidth = i;

            Console.WriteLine(flow.AsStringMap().ToString());
        }
    }
}