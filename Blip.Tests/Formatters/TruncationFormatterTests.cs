using Blip.Formatters;

namespace Blip.Tests.Formatters; 

[TestFixture]
public class TruncationFormatterTests {
    private static object[] AlignmentInvariantSource =  {
        new object[] { Alignment.LEFT },
        new object[] { Alignment.CENTER },
        new object[] { Alignment.RIGHT },
    };

    private readonly int width = 5;
    private readonly int height = 3;
    
    [Test]
    [TestCaseSource(nameof(AlignmentInvariantSource))]
    public void SingleLineEqualBoundsDoesNothing(Alignment alignment) {
        var fmt = new TruncationFormatter(alignment);

        var formatted = fmt.FormatString("panda", this.width, this.height);
        var fmtString = String.Join("", formatted);

        Assert.That(formatted, Has.Length.EqualTo(this.width));
        Assert.That(fmtString,Is.EqualTo("panda"));
    }

    [Test]
    [TestCaseSource(nameof(AlignmentInvariantSource))]
    public void SingleLineGreaterThanBoundsAddsEllipses(Alignment alignment)
    {
        var fmt = new TruncationFormatter(alignment);
        
        char[] formatted = fmt.FormatString("pandas", this.width, this.height);
        string fmtString = String.Join("", formatted);
        
        Assert.Multiple(() =>
        {
            Assert.That(formatted, Has.Length.EqualTo(this.width));
            Assert.That(fmtString, Is.EqualTo("pa..."));
        });
    }

    [Test]
    public void SingleLineLeftAlignmentLessThanBoundsPadsRightSpace()
    {
        var fmt = new TruncationFormatter(Alignment.LEFT);
        
        char[] formatted = fmt.FormatString("dog", this.width, this.height);
        string fmtString = String.Join("", formatted);
        
        Assert.Multiple(() =>
        {
            Assert.That(formatted, Has.Length.EqualTo(this.width));
            Assert.That(fmtString, Is.EqualTo("dog  "));
        });
    }

    [Test]
    public void SingleLineCentreAlignmentLessThanBoundsPadsEqualSpace()
    {
        var fmt = new TruncationFormatter(Alignment.CENTER);
        
        char[] formatted = fmt.FormatString("dog", this.width, this.height);
        string fmtString = String.Join("", formatted);
        
        Assert.Multiple(() =>
        {
            Assert.That(formatted, Has.Length.EqualTo(this.width));
            Assert.That(fmtString, Is.EqualTo(" dog "));
        });
    }

    [Test]
    public void SingleLineRightAlignmentLessThanBoundsPadsLeftSpace()
    {
        var fmt = new TruncationFormatter(Alignment.RIGHT);
        
        char[] formatted = fmt.FormatString("dog", this.width, this.height);
        string fmtString = String.Join("", formatted);
        
        Assert.Multiple(() =>
        {
            Assert.That(formatted, Has.Length.EqualTo(this.width));
            Assert.That(fmtString, Is.EqualTo("  dog"));
        });
    }
    
    
    [Test]
    [TestCaseSource(nameof(AlignmentInvariantSource))]
    public void MultiLineEqualBoundsDoesNothing(Alignment alignment) {
        var fmt = new TruncationFormatter(alignment);

        var formatted = fmt.FormatString("panda\npanda\npanda", this.width, this.height);
        var fmtString = String.Join("", formatted);

        Assert.That(formatted, Has.Length.EqualTo(this.height * this.width));
        Assert.That(fmtString,Is.EqualTo("pandapandapanda"));
    }

    [Test]
    [TestCaseSource(nameof(AlignmentInvariantSource))]
    public void MultiLineGreaterThanBoundsAddsEllipses(Alignment alignment)
    {
        var fmt = new TruncationFormatter(alignment);
        
        char[] formatted = fmt.FormatString("pandas\npandas\npandas", this.width, this.height);
        string fmtString = String.Join("", formatted);
        
        Assert.Multiple(() =>
        {
            Assert.That(formatted, Has.Length.EqualTo(this.height * this.width));
            Assert.That(fmtString, Is.EqualTo("pa...pa...pa..."));
        });
    }
    
    [Test]
    [TestCaseSource(nameof(AlignmentInvariantSource))]
    public void MoreThanAllowedHeightLinesTruncatesLines(Alignment alignment) {
        var fmt = new TruncationFormatter(alignment);

        var formatted = fmt.FormatString("panda\npanda\npanda\npanda", this.width, this.height);
        var fmtString = String.Join("", formatted);

        Assert.That(formatted, Has.Length.EqualTo(this.height * this.width));
        Assert.That(fmtString,Is.EqualTo("pandapandapanda"));
    }
}