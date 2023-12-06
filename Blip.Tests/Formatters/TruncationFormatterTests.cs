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
    public void SingleLineEqualBounds(Alignment alignment) {
        var fmt = new TruncationFormatter(alignment);

        var formatted = fmt.FormatString("panda", this.width, this.height);
        var fmtString = String.Join("", formatted);

        Assert.That(formatted, Has.Length.EqualTo(this.width));
        Assert.That(fmtString,Is.EqualTo("panda"));
    }

    [Test]
    [TestCaseSource(nameof(AlignmentInvariantSource))]
    public void SingleLineGreaterThanBounds(Alignment alignment) {
        var fmt = new TruncationFormatter(alignment);
        
        var formatted = fmt.FormatString("pandas", this.width, this.height);
        var fmtString = String.Join("", formatted);
        
        Assert.That(formatted, Has.Length.EqualTo(this.width));
        Assert.That(fmtString, Is.EqualTo("pa..."));
    }
}