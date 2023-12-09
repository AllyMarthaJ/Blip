using Blip.Diagram.Components;
using Blip.Diagram.Styles;

namespace Blip.Tests.Diagram.Components;

public class BoxTests {
    [Test]
    public void NoTitleNoBorderDoesNotOverflow() {
        var msg = "message";
        var box = new Box(msg)
            { BorderHeight = 0, BorderWidth = 0, MaxWidth = msg.Length * 2, MessagePadding = new Padding() };

        StringMap sm = box.AsStringMap();

        Assert.Multiple(() => {
            Assert.That(sm, Has.Property("Height").EqualTo(1));
            Assert.That(sm, Has.Property("Width").EqualTo(msg.Length * 2));
            Assert.That(sm.ToString(), Does.Contain(msg)); 
        });
    }

    [Test]
    public void NoTitleWithBorderDoesNotOverflow() {
        var msg = "message";
        var box = new Box(msg)
            { BorderHeight = 1, BorderWidth = 1, MaxWidth = msg.Length * 2, MessagePadding = new Padding() };

        StringMap sm = box.AsStringMap();

        Assert.Multiple(() => {
            Assert.That(sm, Has.Property("Height").EqualTo(3));
            Assert.That(sm, Has.Property("Width").EqualTo(msg.Length * 2));
            Assert.That(sm.ToString(), Does.Contain(msg));
        });
    }

    [Test]
    public void TitleAndMessageWithBorderDoesNotOverflow() {
        var title = "title";
        var msg = "message";

        var box = new Box(msg, title) {
            BorderHeight = 1, BorderWidth = 1, MaxWidth = msg.Length * 2, TitlePadding = new Padding(),
            MessagePadding = new Padding()
        };

        StringMap sm = box.AsStringMap();
        var smt = sm.ToString();

        Assert.Multiple(() => {
            Assert.That(sm, Has.Property("Height").EqualTo(5));
            Assert.That(sm, Has.Property("Width").EqualTo(msg.Length * 2));
            Assert.That(smt, Does.Contain(title));
            Assert.That(smt, Does.Contain(msg)); 
        });
    }

    [Test]
    public void TitleAndMessageWithoutBorderDoesNotOverflow() {
        var title = "title";
        var msg = "message";

        var box = new Box(msg, title) {
            BorderHeight = 0, BorderWidth = 0, MaxWidth = msg.Length * 2, TitlePadding = new Padding(),
            MessagePadding = new Padding()
        };

        StringMap sm = box.AsStringMap();
        var smt = sm.ToString();

        Assert.Multiple(() => {
            Assert.That(sm, Has.Property("Height").EqualTo(3));
            Assert.That(sm, Has.Property("Width").EqualTo(msg.Length * 2));
            Assert.That(smt, Does.Contain(title));
            Assert.That(smt, Does.Contain(msg)); 
        });
    }
}