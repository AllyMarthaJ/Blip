using Blip.Format;

namespace Blip.Tests.Formatters;

[TestFixture]
[TestFixtureSource(nameof(StringExtensionTestsSource))]
public class StringExtensionTests(string subStr) {
    private static object[] StringExtensionTestsSource = {
        new object[] { "short" },
        new object[] { "lorem ipsum dolor sit amet" },
        new object[] { "pandas pandas" },
        new object[] { "" },
        new object[] { " " }
    };

    private static object[] AlignmentInvariantTestsSource = {
        new object[] { Alignment.LEFT },
        new object[] { Alignment.RIGHT },
        new object[] { Alignment.CENTER },
        new object[] { Alignment.JUSTIFY }
    };

    [Test]
    public void LeftAlignWithSpaceRemainingPadsSpace() {
        int len = subStr.Length * 2;
        string justified = subStr.Justify(len, Alignment.LEFT);

        Assert.That(justified, Is.EqualTo(subStr + new string(' ', subStr.Length)));
    }

    [Test]
    public void RightAlignWithSpaceRemainingPadsSpace() {
        int len = subStr.Length * 2;
        string justified = subStr.Justify(len, Alignment.RIGHT);

        Assert.That(justified, Is.EqualTo(new string(' ', subStr.Length) + subStr));
    }

    [Test]
    public void CentreAlignWithSpaceRemainingPadsSpace() {
        int len = subStr.Length * 2;

        string justified = subStr.Justify(len, Alignment.CENTER);

        Assert.That(justified,
            Is.EqualTo(new string(' ', subStr.Length / 2) + subStr +
                       new string(' ', subStr.Length - subStr.Length / 2)));
    }

    [Test]
    public void JustifyAlignWithSpaceRemainingInjectsSpace() {
        int len = subStr.Length * 2;

        string justified = subStr.Justify(len, Alignment.JUSTIFY);

        string[] words = subStr.Split(" ");
        int totalWordLength = words
            .Select(w => w.Length)
            .Sum();

        Assert.That(justified.ToCharArray(),
            Has.Exactly(len - totalWordLength).EqualTo(' '));

        if (words.Length == 1 || words.All(string.IsNullOrWhiteSpace)) {
            Assert.That(justified, Is.EqualTo(subStr.Justify(len, Alignment.LEFT)));
        }
        else {
            Assert.That(justified, Is.Not.EqualTo(subStr.Justify(len, Alignment.LEFT)));
        }
    }

    [Test]
    [TestCaseSource(nameof(AlignmentInvariantTestsSource))]
    public void AlignWithTooLittleSpaceDoesNothing(Alignment alignment) {
        int len = subStr.Length / 2;

        Assert.That(subStr.Justify(len, alignment), Is.EqualTo(subStr));
    }


    [Test]
    [TestCaseSource(nameof(AlignmentInvariantTestsSource))]
    public void AlignWithEqualSpaceDoesNothing(Alignment alignment) {
        int len = subStr.Length;

        Assert.That(subStr.Justify(len, alignment), Is.EqualTo(subStr));
    }
}