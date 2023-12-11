using Blip.Diagram.Components;
using Blip.Diagram.Styles;
using Blip.Format;

namespace Blip.Tests.Diagram.Components;

public class FlowTests {
    private static object[] InvariantTestsSource = {
        new object[] { Direction.HORIZONTAL, Alignment.LEFT },
        new object[] { Direction.HORIZONTAL, Alignment.CENTER },
        new object[] { Direction.HORIZONTAL, Alignment.RIGHT },
        new object[] { Direction.HORIZONTAL, Alignment.JUSTIFY },
        new object[] { Direction.VERTICAL, Alignment.LEFT },
        new object[] { Direction.VERTICAL, Alignment.CENTER },
        new object[] { Direction.VERTICAL, Alignment.RIGHT },
        new object[] { Direction.VERTICAL, Alignment.JUSTIFY },
    };

    [Test]
    [TestCaseSource(nameof(InvariantTestsSource))]
    public void NoPrimarySecondaryAxesGeneratesSingleLine(Direction dir, Alignment flowAlignment) {
        Text[] children = Enumerable.Repeat(new Text(Alignment.LEFT, "a"), 10).ToArray();

        var flow = new Flow { Children = children, FlowDirection = dir, FlowAlignment = flowAlignment };
        StringMap sm = flow.AsStringMap();

        switch (dir) {
            case Direction.HORIZONTAL:
                Assert.That(sm, Has.Property("Width").EqualTo(children.Length + flow.ChildGap * (children.Length - 1)));
                Assert.That(sm, Has.Property("Height").EqualTo(1));
                break;
            case Direction.VERTICAL:
                Assert.That(sm,
                    Has.Property("Height").EqualTo(children.Length + flow.ChildGap * (children.Length - 1)));
                Assert.That(sm, Has.Property("Width").EqualTo(1));
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(dir), dir, null);
        }

        Assert.That(sm.ToString(), Has.Exactly(10).EqualTo('a'));
    }

    [Test]
    [TestCaseSource(nameof(InvariantTestsSource))]
    public void ExcessPrimaryAxisNoSecondaryAxisGeneratesSingleLine(Direction dir, Alignment flowAlignment) {
        Text[] children = Enumerable.Repeat(new Text(Alignment.LEFT, "a"), 10).ToArray();

        var flow = new Flow { Children = children, FlowDirection = dir, FlowAlignment = flowAlignment };
        switch (dir) {
            case Direction.HORIZONTAL:
                flow.MaxWidth = children.Length * 10;
                break;
            case Direction.VERTICAL:
                flow.MaxHeight = children.Length * 10;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(dir), dir, null);
        }

        StringMap sm = flow.AsStringMap();

        switch (dir) {
            case Direction.HORIZONTAL:
                Assert.That(sm, Has.Property("Width").EqualTo(flow.MaxWidth));
                Assert.That(sm, Has.Property("Height").EqualTo(1));
                break;
            case Direction.VERTICAL:
                Assert.That(sm, Has.Property("Height").EqualTo(flow.MaxHeight));
                Assert.That(sm, Has.Property("Width").EqualTo(1));
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(dir), dir, null);
        }

        Assert.That(sm.ToString(), Has.Exactly(10).EqualTo('a'));
    }

    [Test]
    [TestCaseSource(nameof(InvariantTestsSource))]
    public void LimitedPrimaryAxisNoSecondaryAxisGeneratesMultipleLines(Direction dir, Alignment flowAlignment) {
        Text[] children = Enumerable.Repeat(new Text(Alignment.LEFT, "a"), 10).ToArray();

        var flow = new Flow { Children = children, FlowDirection = dir, FlowAlignment = flowAlignment };

        var maxAxis = children.Length + (children.Length - 1) * flow.ChildGap;
        switch (dir) {
            case Direction.HORIZONTAL:
                flow.MaxWidth = maxAxis;
                break;
            case Direction.VERTICAL:
                flow.MaxHeight = maxAxis;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(dir), dir, null);
        }
        
        while (flow.MaxWidth > 1) {
            var sm = flow.AsStringMap();

            var expectedChildrenPerRow = (int)Math.Ceiling(flow.MaxWidth / (double)maxAxis * children.Length);
            var expectedRows = (int)Math.Ceiling(children.Length / (double)expectedChildrenPerRow);

            var expectedPrimaryAxisSize = expectedChildrenPerRow + (expectedChildrenPerRow - 1) * flow.ChildGap;
            var expectedSecondaryAxisSize = expectedRows + (expectedRows - 1) * flow.RowGap;
            
            Console.WriteLine($"With width {flow.MaxWidth} expect {expectedChildrenPerRow} children per row on {expectedRows} rows - real {sm.Width}");
            switch (dir) {
                case Direction.HORIZONTAL:
                    Assert.That(sm, Has.Property("Width").EqualTo(expectedPrimaryAxisSize));
                    Assert.That(sm, Has.Property("Height").EqualTo(expectedSecondaryAxisSize));
                    break;
                case Direction.VERTICAL:
                    Assert.That(sm, Has.Property("Height").EqualTo(expectedPrimaryAxisSize));
                    Assert.That(sm, Has.Property("Width").EqualTo(expectedSecondaryAxisSize));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(dir), dir, null);
            }
            
            Assert.That(sm.ToString(), Has.Exactly(10).EqualTo('a'));
            
            flow.MaxWidth--;
        }
        switch (dir) {
            case Direction.HORIZONTAL:
                flow.MaxWidth = children.Length * 10;
                break;
            case Direction.VERTICAL:
                flow.MaxHeight = children.Length * 10;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(dir), dir, null);
        }
    }
}