using Blip.Diagram.Components;
using Blip.Diagram.Styles;
using Blip.Format;

namespace Blip.Tests.Diagram.Components;

public class FlowTests {
    [Test]
    public void FlowBlah() {
        var children = new List<IDiagramComponent>();
        Flow flow = new() { MaxHeight = 11, Children = children, FlowAlignment = Alignment.JUSTIFY, FlowDirection = Direction.VERTICAL};

        for (int i = 0; i < 20; i++) {
            children.Add(new Text(Alignment.LEFT, i % 2 == 0 ? "hi" : "bye"));
        }

        for (int i = 1; i < 20; i += 3) {
            Console.WriteLine($"With width {i} ---------------");
            flow.MaxHeight = i;

            Console.WriteLine(flow.AsStringMap().ToString());
        }
    }
}