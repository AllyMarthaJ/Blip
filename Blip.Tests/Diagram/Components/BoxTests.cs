using Blip.Diagram.Components;

namespace Blip.Tests.Diagram.Components;

public class BoxTests {
    [Test]
    public void NoTitleNoBorderDoesNotOverflow() {
        var msg = "message";
        var box = new Box(msg) { BorderHeight = 0, BorderWidth = 0, MaxWidth = msg.Length * 2, MessagePadding = new() };

        var sm = box.AsStringMap();

        Assert.That(sm, Has.Property("Height").EqualTo(1));
        Assert.That(sm, Has.Property("Width").EqualTo(msg.Length * 2));
        Assert.That(sm.ToString(), Does.Contain(msg));
    }

    [Test]
    public void NoTitleWithBorderDoesNotOverflow() {
        var msg = "message";
        var box = new Box(msg) { BorderHeight = 1, BorderWidth = 1, MaxWidth = msg.Length * 2, MessagePadding = new() };

        var sm = box.AsStringMap();

        Assert.That(sm, Has.Property("Height").EqualTo(3));
        Assert.That(sm, Has.Property("Width").EqualTo(msg.Length * 2));
        Assert.That(sm.ToString(), Does.Contain(msg));
    }
}