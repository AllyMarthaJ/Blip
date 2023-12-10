using Blip.Diagram;
using Blip.Diagram.Components;

namespace Blip.Tests.Diagram.Components;

public class RawTests {
    private int smWidth = 5;
    private int smHeight = 5;

    private StringMap sm;

    [SetUp]
    public void Setup() {
        this.sm = StringMap.FromLineDelimitedString(
            String.Join("\n", Enumerable.Repeat(
                String.Join("", Enumerable.Repeat('#', this.smWidth)),
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
        var width = this.smWidth - 1;

        Raw rawComponent = new(this.sm) { MaxWidth = width };

        var smt = rawComponent.AsStringMap();

        Assert.Multiple(() => {
            Assert.That(smt, Is.Not.EqualTo(this.sm));

            Assert.That(smt, Has.Property("Width").EqualTo(width));
            Assert.That(smt, Has.Property("Height").EqualTo(this.smHeight));

            for (int y = 0; y < this.smHeight; y++) {
                for (int x = 0; x < width; x++) {
                    Assert.That(smt.GetChar(x, y), Is.EqualTo(this.sm.GetChar(x, y)));
                }
            }
        });
    }

    [Test]
    public void RawComponentWithSmallerHeightReturnsCropped() {
        var height = this.smHeight - 1;

        Raw rawComponent = new(this.sm) { MaxHeight = height };

        var smt = rawComponent.AsStringMap();

        Assert.Multiple(() => {
            Assert.That(smt, Is.Not.EqualTo(this.sm));

            Assert.That(smt, Has.Property("Width").EqualTo(this.smWidth));
            Assert.That(smt, Has.Property("Height").EqualTo(height));

            for (int y = 0; y < height; y++) {
                for (int x = 0; x < this.smWidth; x++) {
                    Assert.That(smt.GetChar(x, y), Is.EqualTo(this.sm.GetChar(x, y)));
                }
            }
        });
    }
    

    [Test]
    public void RawComponentWithLargerWidthReturnsOriginalStringMap() {
        var width = this.smWidth + 1;

        Raw rawComponent = new(this.sm) { MaxWidth = width };

        Assert.That(rawComponent.AsStringMap(), Is.EqualTo(this.sm));
    }

    [Test]
    public void RawComponentWithLargerHeightReturnsOriginalStringMap() {
        var height = this.smHeight + 1;

        Raw rawComponent = new(this.sm) { MaxHeight = height };

        Assert.That(rawComponent.AsStringMap(), Is.EqualTo(this.sm));
    }
}