using Blip.Diagram.Components;
using Blip.Format;

namespace Blip.Tests.Diagram.Components;

public class FrameTests {
    private StringMap sm;

    [SetUp]
    public void Setup() {
        this.sm = new StringMap(1, 1).FillRectangle('#', 0, 0, 1, 1);
    }

    private static object[] AlignmentTestsSource = {
        new object[] { Alignment.LEFT, Alignment.LEFT },
        new object[] { Alignment.CENTER, Alignment.LEFT },
        new object[] { Alignment.RIGHT, Alignment.LEFT },
        new object[] { Alignment.LEFT, Alignment.CENTER },
        new object[] { Alignment.CENTER, Alignment.CENTER },
        new object[] { Alignment.RIGHT, Alignment.CENTER },
        new object[] { Alignment.LEFT, Alignment.RIGHT },
        new object[] { Alignment.CENTER, Alignment.RIGHT },
        new object[] { Alignment.RIGHT, Alignment.RIGHT },
    };

    [Test]
    [TestCaseSource(nameof(AlignmentTestsSource))]
    public void AlignsTheSourceWhenFrameLargerThanStringMap(Alignment left, Alignment top) {
        var contentLeft = left switch {
            Alignment.LEFT => 0,
            Alignment.CENTER => 1,
            Alignment.RIGHT => 2,
            _ => throw new ArgumentOutOfRangeException(nameof(left), left, null)
        };

        var contentTop = top switch {
            Alignment.LEFT => 0,
            Alignment.CENTER => 1,
            Alignment.RIGHT => 2,
            _ => throw new ArgumentOutOfRangeException(nameof(top), top, null)
        };

        var frame = new Frame() {
            Children = new[] { new Raw(this.sm) },
            MaxWidth = 3,
            MaxHeight = 3,
            HorizontalAlignment = left,
            VerticalAlignment = top
        };
        var smt = frame.AsStringMap();

        for (int y = 0; y < 2; y++) {
            for (int x = 0; x < 2; x++) {
                if (x == contentLeft && y == contentTop) {
                    Assert.That(smt.GetChar(x, y), Is.EqualTo(this.sm.GetChar(0, 0)));
                }
                else {
                    Assert.That(smt.GetChar(x, y), Is.EqualTo(' '));
                }
            }
        }
    }

    [Test]
    public void CropsTheSourceWhenFrameSmallerThanStringMap() {
        var frame = new Frame {
            Children = new[] {
                new Raw(
                    new StringMap(2, 2)
                        .FillRectangle('#', 0, 0, 2, 2))
            },
            MaxWidth = 1,
            MaxHeight = 1,
        };
        var smt = frame.AsStringMap();

        Assert.That(smt.Width, Is.EqualTo(1));
        Assert.That(smt.Height, Is.EqualTo(1));
        Assert.That(smt.GetChar(0,0), Is.EqualTo('#'));
    }

    [Test]
    public void ReturnsTheSourceWhenFrameIsTheSameSizeAsStringMap() {
        var frame = new Frame {
            Children = new[] { new Raw(this.sm) },
            MaxWidth = this.sm.Width,
            MaxHeight = this.sm.Height
        };
        var smt = frame.AsStringMap();

        Assert.That(smt, Is.EqualTo(this.sm));
    }
    
    [Test]
    public void ReturnsTheSourceWhenFrameHasNoMaximumSize() {
        var frame = new Frame {
            Children = new[] { new Raw(this.sm) },
        };
        var smt = frame.AsStringMap();

        Assert.That(smt, Is.EqualTo(this.sm));
    }
}