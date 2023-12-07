using Blip.Transforms;

namespace Blip.Tests.Transforms;

[TestFixture]
public class UnionTransformTests {
    [SetUp]
    public void Setup() { }

    private static readonly char[] PreserveOriginTestsBlank = { ' ', '.' };

    private static object[] PreserveOriginTestsSource = {
        // Only one-of are blank.
        new object[] { 'x', PreserveOriginTestsBlank.First(), 'x', 'x' },
        // Both are blank.
        new object[] {
            PreserveOriginTestsBlank[0], PreserveOriginTestsBlank[1], PreserveOriginTestsBlank[0],
            PreserveOriginTestsBlank[1]
        },
        // Neither are blank; preserve the left.
        new object[] { 'x', 'y', 'x', 'y' }
    };

    [Test]
    [TestCaseSource(nameof(PreserveOriginTestsSource))]
    public void UnionPreserveOriginTests(char left, char right, char expectedLeft, char expectedRight) {
        var transform = new UnionTransform(new UnionTransformOptions
            { PreserveTarget = PreservationMode.ORIGIN, Blank = PreserveOriginTestsBlank });

        // Tests Faux-commutativity.
        Assert.Multiple(() => {
            Assert.That(transform.Transform(left, right), Is.EqualTo(expectedLeft));
            Assert.That(transform.Transform(right, left), Is.EqualTo(expectedRight));
        });
    }

    private static readonly char[] PreserveDestTestsBlank = { ' ', '.' };

    private static object[] PreserveDestTestsSource = {
        // Only one-of are blank.
        new object[] { 'x', PreserveDestTestsBlank.First(), 'x', 'x' },
        // Both are blank.
        new object[] {
            PreserveDestTestsBlank[0], PreserveDestTestsBlank[1], PreserveDestTestsBlank[1], PreserveDestTestsBlank[0]
        },
        // Neither are blank; preserve the right.
        new object[] { 'x', 'y', 'y', 'x' }
    };

    [Test]
    [TestCaseSource(nameof(PreserveDestTestsSource))]
    public void UnionPreserveDestTests(char left, char right, char expectedLeft, char expectedRight) {
        var transform = new UnionTransform(new UnionTransformOptions
            { PreserveTarget = PreservationMode.DESTINATION, Blank = PreserveDestTestsBlank });

        // Tests Faux-commutativity.
        Assert.Multiple(() => {
            Assert.That(transform.Transform(left, right), Is.EqualTo(expectedLeft));
            Assert.That(transform.Transform(right, left), Is.EqualTo(expectedRight));
        });
    }
}