using Blip.Diagram.Components;
using Blip.Format;

namespace Blip.Tests.Diagram.Components;

public class TreeTests {
    private static object[] UnaryNodeSources = new[] {
        new object[] { new Text(Alignment.LEFT, "pandas"), new[] { new Text(Alignment.LEFT, "aa") } },
        new object[] { new Text(Alignment.LEFT, "pandas"), new[] { new Text(Alignment.LEFT, "a") } },
        new object[] { new Text(Alignment.LEFT, "panda"), new[] { new Text(Alignment.LEFT, "aa") } },
        new object[] { new Text(Alignment.LEFT, "panda"), new[] { new Text(Alignment.LEFT, "a") } },

        new object[] { new Text(Alignment.LEFT, "aa"), new[] { new Text(Alignment.LEFT, "pandas") } },
        new object[] { new Text(Alignment.LEFT, "a"), new[] { new Text(Alignment.LEFT, "pandas") } },
        new object[] { new Text(Alignment.LEFT, "aa"), new[] { new Text(Alignment.LEFT, "panda") } },
        new object[] { new Text(Alignment.LEFT, "a"), new[] { new Text(Alignment.LEFT, "panda") } },
    };


    [Test]
    [TestCaseSource(nameof(UnaryNodeSources))]
    public void UnaryTreeShouldNotContainDuplicateEdges(Text baseNode, params IDiagramComponent[] nodes) {
        var tree = new Tree(
            baseNode, nodes
        );

        var sm = tree.AsStringMap();
        var smt = sm.ToString();

        // Unary node misalignment checks
        Assert.Multiple(() => {
            Assert.That(smt, Does.Not.Contain("||"));
            Assert.That(smt, Does.Not.Contain("| |"));

            for (int y = 0; y < sm.Height; y++) {
                for (int x = 0; x < sm.Width; x++) {
                    if (sm.GetChar(x, y) == '|') {
                        if (y > 0) {
                            Assert.That(sm.GetChar(x, y - 1), Is.Not.EqualTo(' '));
                        }

                        if (y < sm.Height - 1) {
                            Assert.That(sm.GetChar(x, y + 1), Is.Not.EqualTo(' '));
                        }
                    }
                }
            }
        });
    }
}