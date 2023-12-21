using System.Xml;
using Blip.Diagram.Components;
using Blip.Diagram.Styles;
using Blip.Format;
using ExtendedXmlSerializer;
using ExtendedXmlSerializer.Configuration;

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

        StringMap sm = flow.AsStringMap();

        Assert.That(sm.ToString(), Has.Exactly(9).EqualTo('a'));
    }

    [Test]
    public void BlahAgain() {
        List<IDiagramComponent> documentChildren = new();
        Flow document = new()
            { FlowDirection = Direction.VERTICAL, RowAlignment = Alignment.CENTER, Children = documentChildren };

        List<IDiagramComponent> titleChildren = new();
        Flow titleFlow = new()
            { Children = titleChildren, RowAlignment = Alignment.CENTER, ChildGap = 10 };

        Box titleBox = new("Authored by: Ally Martha\n Date: 12/12/23", "Lorem Ipsum")
            { MessageAlignment = Alignment.LEFT };
        titleChildren.Add(titleBox);

        var exampleTree = new Tree(
            new Text(Alignment.LEFT, "lorem"),
            new Tree(new Text(Alignment.LEFT, "ipsum"),
                new Tree(
                    new Box("panda") {
                        MessagePadding = new Padding(), MaxWidth = 10, MaxHeight = 3,
                        MessageAlignment = Alignment.CENTER
                    },
                    new Tree(new Text(Alignment.LEFT, "black")),
                    new Tree(new Text(Alignment.LEFT, "white"))
                ),
                new Tree(new Text(Alignment.LEFT, "dog"),
                    new Tree(new Text(Alignment.LEFT, "floof"))
                )
            ),
            new Tree(new Text(Alignment.LEFT, "at")),
            new Tree(new Text(Alignment.LEFT, "eros"))
        );
        titleChildren.Add(exampleTree);

        documentChildren.Add(titleFlow);

        var documentText = new Text(Alignment.JUSTIFY,
                "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aliquam nunc sapien, lacinia vitae hendrerit non, tincidunt sit amet nunc. Nunc laoreet mauris ac feugiat mattis. Quisque mattis dui vel sagittis semper. Morbi finibus hendrerit odio at elementum. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia curae; Nunc hendrerit nulla id neque sollicitudin ultricies. Interdum et malesuada fames ac ante ipsum primis in faucibus. Donec varius cursus eleifend. Donec congue neque ut faucibus tristique. Donec quis tempus orci, et rhoncus mauris. Etiam sit amet viverra ipsum.\n\nProin cursus est sed arcu lacinia, eu pulvinar quam tempor. Ut sit amet accumsan diam. Ut ut lorem at elit rhoncus dapibus ac sit amet dui. Sed eget ex tincidunt, condimentum nisl quis, faucibus ipsum. Phasellus vehicula pharetra eleifend. Integer mauris sem, eleifend id turpis ac, efficitur aliquet purus. Maecenas facilisis consequat sapien, vitae euismod ligula consequat vitae. Praesent faucibus tempor vulputate. Integer at tortor sit amet risus luctus bibendum. Integer tristique bibendum diam, at bibendum nunc fringilla eget. Aliquam in purus a libero lobortis condimentum ac eget nisi. Maecenas sed luctus neque. Suspendisse laoreet augue quis finibus sagittis. Nunc convallis enim in gravida luctus. Fusce gravida a felis ut finibus. Proin maximus mattis lacus, et semper enim mattis vitae.\n\nSed et augue interdum, euismod lacus sit amet, tincidunt odio. Morbi interdum nibh nec metus venenatis dictum. Pellentesque at risus imperdiet, ornare ante in, dictum lacus. Integer purus diam, mattis non interdum et, malesuada et tellus. Suspendisse luctus consectetur erat at cursus. Praesent semper auctor mauris quis scelerisque. Etiam eget condimentum ex. Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos himenaeos. Nullam lorem eros, commodo non laoreet nec, finibus porta mi. Phasellus eleifend eu nisl et mollis. Quisque ut vulputate ante. Quisque a libero eget sem dapibus ornare sed vel libero. Integer gravida, sem vitae euismod efficitur, enim nunc faucibus neque, vitae porta nisi tellus sit amet lorem. Pellentesque eu augue eget ex placerat tempor. ")
            { MaxWidth = 100 };
        documentChildren.Add(documentText);

        // Console.WriteLine(f.AsStringMap());

        IExtendedXmlSerializer serializer = new
                ConfigurationContainer()
            .UseAutoFormatting()
            .UseOptimizedNamespaces()
            .EnableReferences()
            .EnableImplicitTyping(typeof(IDiagramComponent), typeof(Box), typeof(Flow), typeof(Frame), typeof(Text),
                typeof(Tree))
            .Create();

        var serial = serializer.Serialize(new XmlWriterSettings { Indent = true }, document);
        var obj = serializer.Deserialize<IDiagramComponent>(serial!);
        Console.WriteLine(serial);
    }
}