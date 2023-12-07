using Blip.Formatters;
using Blip.Transforms;

namespace Blip.Tests.Formatters;

[TestFixture]
public class TruncationFormatterTests {
    private static object[] AlignmentInvariantSource = {
        new object[] { Alignment.LEFT },
        new object[] { Alignment.CENTER },
        new object[] { Alignment.RIGHT }
    };

    private readonly int width = 5;
    private readonly int height = 3;

    [Test]
    [TestCaseSource(nameof(AlignmentInvariantSource))]
    public void SingleLineEqualBoundsDoesNothing(Alignment alignment) {
        var fmt = new TruncationFormatter(alignment);

        char[] formatted = fmt.FormatString("panda", this.width, this.height);
        string fmtString = String.Join("", formatted);

        Assert.That(formatted, Has.Length.EqualTo(this.width));
        Assert.That(fmtString, Is.EqualTo("panda"));
    }

    [Test]
    [TestCaseSource(nameof(AlignmentInvariantSource))]
    public void SingleLineGreaterThanBoundsAddsEllipses(Alignment alignment) {
        var fmt = new TruncationFormatter(alignment);

        char[] formatted = fmt.FormatString("pandas", this.width, this.height);
        string fmtString = String.Join("", formatted);

        Assert.Multiple(() => {
            Assert.That(formatted, Has.Length.EqualTo(this.width));
            Assert.That(fmtString, Is.EqualTo("pa..."));
        });
    }

    [Test]
    public void SingleLineLeftAlignmentLessThanBoundsPadsRightSpace() {
        var fmt = new TruncationFormatter(Alignment.LEFT);

        char[] formatted = fmt.FormatString("dog", this.width, this.height);
        string fmtString = String.Join("", formatted);

        Assert.Multiple(() => {
            Assert.That(formatted, Has.Length.EqualTo(this.width));
            Assert.That(fmtString, Is.EqualTo("dog  "));
        });
    }

    [Test]
    public void SingleLineCentreAlignmentLessThanBoundsPadsEqualSpace() {
        var fmt = new TruncationFormatter(Alignment.CENTER);

        char[] formatted = fmt.FormatString("dog", this.width, this.height);
        string fmtString = String.Join("", formatted);

        Assert.Multiple(() => {
            Assert.That(formatted, Has.Length.EqualTo(this.width));
            Assert.That(fmtString, Is.EqualTo(" dog "));
        });
    }

    [Test]
    public void SingleLineRightAlignmentLessThanBoundsPadsLeftSpace() {
        var fmt = new TruncationFormatter(Alignment.RIGHT);

        char[] formatted = fmt.FormatString("dog", this.width, this.height);
        string fmtString = String.Join("", formatted);

        Assert.Multiple(() => {
            Assert.That(formatted, Has.Length.EqualTo(this.width));
            Assert.That(fmtString, Is.EqualTo("  dog"));
        });
    }


    [Test]
    [TestCaseSource(nameof(AlignmentInvariantSource))]
    public void MultiLineEqualBoundsDoesNothing(Alignment alignment) {
        var fmt = new TruncationFormatter(alignment);

        char[] formatted = fmt.FormatString("panda\npanda\npanda", this.width, this.height);
        string fmtString = String.Join("", formatted);

        Assert.That(formatted, Has.Length.EqualTo(this.height * this.width));
        Assert.That(fmtString, Is.EqualTo("pandapandapanda"));
    }

    [Test]
    [TestCaseSource(nameof(AlignmentInvariantSource))]
    public void MultiLineGreaterThanBoundsAddsEllipses(Alignment alignment) {
        var fmt = new TruncationFormatter(alignment);

        char[] formatted = fmt.FormatString("pandas\npandas\npandas", this.width, this.height);
        string fmtString = String.Join("", formatted);

        Assert.Multiple(() => {
            Assert.That(formatted, Has.Length.EqualTo(this.height * this.width));
            Assert.That(fmtString, Is.EqualTo("pa...pa...pa..."));
        });
    }

    [Test]
    [TestCaseSource(nameof(AlignmentInvariantSource))]
    public void MoreThanAllowedHeightLinesTruncatesLines(Alignment alignment) {
        var fmt = new TruncationFormatter(alignment);

        char[] formatted = fmt.FormatString("panda\npanda\npanda\npanda", this.width, this.height);
        string fmtString = String.Join("", formatted);

        Assert.That(formatted, Has.Length.EqualTo(this.height * this.width));
        Assert.That(fmtString, Is.EqualTo("pandapandapanda"));
    }

    [Test]
    public void Blah() {
        var w = 30;
        var h = 5;
        var str =
            "Ally is an amazing panda who loves to do maths and panda all day long. She really likes pandas and loves bunnies so much, you know?";
        var opts = new UnionTransform(new UnionTransformOptions
            { Blank = new[] { ' ' }, PreserveTarget = PreservationMode.DESTINATION });
        StringMap background =
            new StringMap(w + 2, h + 2)
                .FillRectangle('+', 0, 0, w + 2, h + 2);
        StringMap sm =
            new StringMap(w + 2, h + 2)
                .DrawRectangle('#', 0, 0, w + 2, h + 2)
                .DrawString(str, new WordSplitFormatter(Alignment.CENTER), 1, 1, w, h);
                // .DrawStringMap(background, opts, 0, 0, w + 2, h + 2);

        Console.WriteLine(sm);
    }
}