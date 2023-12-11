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
        new object[] { Direction.VERTICAL, Alignment.JUSTIFY }
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

        int maxAxis = children.Length + (children.Length - 1) * flow.ChildGap;

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

        int pAxis = dir switch {
            Direction.HORIZONTAL => flow.MaxWidth,
            Direction.VERTICAL => flow.MaxHeight,
            _ => throw new ArgumentOutOfRangeException(nameof(dir), dir, null)
        };

        // Testing without recomputing the layout here is a PITA.
        // Test that all values are present, and that some semblance
        // of layout is preserved.
        while (pAxis > 1) {
            StringMap sm = flow.AsStringMap();

            Assert.That(sm.ToString(), Has.Exactly(10).EqualTo('a'));
            switch (dir) {
                case Direction.HORIZONTAL: {
                    Assert.That(sm, Has.Property("Width").EqualTo(flow.MaxWidth));
                    Assert.That(sm, Has.Property("Width").GreaterThan(flow.MaxHeight));
                    flow.MaxWidth--;
                    break;
                }
                case Direction.VERTICAL: {
                    Assert.That(sm, Has.Property("Height").EqualTo(flow.MaxHeight));
                    Assert.That(sm, Has.Property("Height").GreaterThan(flow.MaxWidth));
                    flow.MaxHeight--;
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException(nameof(dir), dir, null);
            }

            pAxis--;
        }
    }

    [Test]
    [TestCaseSource(nameof(InvariantTestsSource))]
    public void LimitedPrimaryAxisLimitedSecondaryAxisTruncates(Direction dir, Alignment flowAlignment) {
        Text[] children = Enumerable.Repeat(new Text(Alignment.LEFT, "a"), 10).ToArray();

        var flow = new Flow { Children = children, FlowDirection = dir, FlowAlignment = flowAlignment };

        int maxAxis = children.Length + (children.Length - 1) * flow.ChildGap - 2;

        switch (dir) {
            case Direction.HORIZONTAL:
                flow.MaxWidth = maxAxis;
                flow.MaxHeight = 1;
                break;
            case Direction.VERTICAL:
                flow.MaxHeight = maxAxis;
                flow.MaxWidth = 1;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(dir), dir, null);
        }

        var sm = flow.AsStringMap();

        Assert.That(sm.ToString(), Has.Exactly(9).EqualTo('a'));
    }

    [Test]
    public void Stuff() {
        Raw r1 = new Raw(StringMap.FromLineDelimitedString("#####\n#####\n#####\n#####\n#####"));
        Raw r2 = new Raw(StringMap.FromLineDelimitedString("###\n###\n###"));
        Raw r3 = new Raw(StringMap.FromLineDelimitedString("#"));
        Raw r4 = new Raw(StringMap.FromLineDelimitedString("###\n###\n###"));
        Raw r5 = new Raw(StringMap.FromLineDelimitedString("##\n##"));
        Raw r6 = new Raw(StringMap.FromLineDelimitedString("#"));


        Flow f = new() {
            Children = new[] { r1, r2, r3, r4, r5, r6 }, RowAlignment = Alignment.CENTER,
            FlowAlignment = Alignment.JUSTIFY, MaxWidth = 11
        };

        Console.WriteLine(f.AsStringMap());
    }
}