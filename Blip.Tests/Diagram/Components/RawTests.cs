using Blip.Diagram.Components;

namespace Blip.Tests.Diagram.Components;

public class RawTests {
    private StringMap sm;
    private readonly int smHeight = 5;
    private readonly int smWidth = 5;

    [SetUp]
    public void Setup() {
        this.sm = StringMap.FromLineDelimitedString(
            string.Join("\n", Enumerable.Repeat(
                string.Join("", Enumerable.Repeat('#', this.smWidth)),
                this.smHeight
            ))
        );
    }

    [Test]
    public void RawComponentWithoutWidthOrHeightReturnsOriginalStringMap() {
        Raw rawComponent = new(this.sm);

        Assert.That(rawComponent.AsStringMap(), Is.EqualTo(this.sm));
    }

    [Test]
    public void RawComponentWithSmallerWidthReturnsCropped() {
        int width = this.smWidth - 1;

        Raw rawComponent = new(this.sm) { MaxWidth = width };

        StringMap smt = rawComponent.AsStringMap();

        Assert.Multiple(() => {
            Assert.That(smt, Is.Not.EqualTo(this.sm));

            Assert.That(smt, Has.Property("Width").EqualTo(width));
            Assert.That(smt, Has.Property("Height").EqualTo(this.smHeight));

            for (var y = 0; y < this.smHeight; y++) {
                for (var x = 0; x < width; x++) {
                    Assert.That(smt.GetChar(x, y), Is.EqualTo(this.sm.GetChar(x, y)));
                }
            }
        });
    }

    [Test]
    public void RawComponentWithSmallerHeightReturnsCropped() {
        int height = this.smHeight - 1;

        Raw rawComponent = new(this.sm) { MaxHeight = height };

        StringMap smt = rawComponent.AsStringMap();

        Assert.Multiple(() => {
            Assert.That(smt, Is.Not.EqualTo(this.sm));

            Assert.That(smt, Has.Property("Width").EqualTo(this.smWidth));
            Assert.That(smt, Has.Property("Height").EqualTo(height));

            for (var y = 0; y < height; y++) {
                for (var x = 0; x < this.smWidth; x++) {
                    Assert.That(smt.GetChar(x, y), Is.EqualTo(this.sm.GetChar(x, y)));
                }
            }
        });
    }


    [Test]
    public void RawComponentWithLargerWidthReturnsOriginalStringMap() {
        int width = this.smWidth + 1;

        Raw rawComponent = new(this.sm) { MaxWidth = width };

        Assert.That(rawComponent.AsStringMap(), Is.EqualTo(this.sm));
    }

    [Test]
    public void RawComponentWithLargerHeightReturnsOriginalStringMap() {
        int height = this.smHeight + 1;

        Raw rawComponent = new(this.sm) { MaxHeight = height };

        Assert.That(rawComponent.AsStringMap(), Is.EqualTo(this.sm));
    }
}