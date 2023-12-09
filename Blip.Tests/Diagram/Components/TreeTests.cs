using Blip.Diagram.Components;
using Blip.Format;

namespace Blip.Tests.Diagram.Components;

public class TreeTests {
    private static object[] UnaryNodeSources = new[] {
        new object[] { new[] { new Text(Alignment.LEFT, "pandas"), new Text(Alignment.LEFT, "aaa") } },
        new object[] { new[] { new Text(Alignment.LEFT, "pandas"), new Text(Alignment.LEFT, "aa") } },
        new object[] { new[] { new Text(Alignment.LEFT, "pandas"), new Text(Alignment.LEFT, "a") } },
        new object[] { new[] { new Text(Alignment.LEFT, "panda"), new Text(Alignment.LEFT, "aaa") } },
        new object[] { new[] { new Text(Alignment.LEFT, "panda"), new Text(Alignment.LEFT, "aa") } },
        new object[] { new[] { new Text(Alignment.LEFT, "panda"), new Text(Alignment.LEFT, "a") } },
        new object[] { new[] { new Text(Alignment.LEFT, "aaa"), new Text(Alignment.LEFT, "aaa") } },
        new object[] { new[] { new Text(Alignment.LEFT, "aaaa"), new Text(Alignment.LEFT, "aaaa") } },
    };

    [Test]
    public void Blah() {
        var maxNodes = 40;
        var rnd = new Random();
        Console.WriteLine("----TREE----");
        Console.WriteLine(this.randomTree(rnd, ref maxNodes, maxBranchingFactor: 4, maxDepth: 3).AsStringMap());
        Console.WriteLine(
            new Tree(
                new Text(Alignment.LEFT, "pandas"),
                new Text(Alignment.LEFT, "a")
            ) { ParentSpacing = 3 }.AsStringMap()
        );

        Console.WriteLine(
            new Tree(
                new Text(Alignment.LEFT, "a"),
                new Text(Alignment.LEFT, "pandas")
            ) { ParentSpacing = 3 }.AsStringMap()
        );

        Console.WriteLine(
            new Tree(
                new Text(Alignment.LEFT, "panda"),
                new Text(Alignment.LEFT, "aa")
            ) { ParentSpacing = 3 }.AsStringMap()
        );

        Console.WriteLine(
            new Tree(
                new Text(Alignment.LEFT, "aa"),
                new Text(Alignment.LEFT, "panda")
            ) { ParentSpacing = 3 }.AsStringMap()
        );
    }

    private Tree randomTree(
        Random rnd,
        ref int maxNodes,
        int depth = 1,
        int maxDepth = 3,
        int maxBranchingFactor = 3,
        int idx = 0
    ) {
        var r = new[] { "mao", "peter", "bun", "qc", "ally", "panda", "mouse", "honman", "honmouse" };
        var t = new Tree(new Text(Alignment.LEFT, r[rnd.Next(r.Length)])) { SiblingSpacing = 2, ParentSpacing = 1 };
        maxNodes -= 1;

        if (maxNodes <= 1 || depth == maxDepth) {
            return t;
        }

        // How many children should this tree have?
        Tree[] children = new Tree[Math.Min(rnd.Next(maxNodes), maxBranchingFactor)];

        bool reverse = rnd.Next(2) == 0;
        if (reverse) {
            for (int i = children.Length - 1; i >= 0; i--) {
                children[i] = this.randomTree(rnd, ref maxNodes, depth + 1, maxDepth, maxBranchingFactor, i);
            }
        }
        else {
            for (var i = 0; i < children.Length; i++) {
                children[i] = this.randomTree(rnd, ref maxNodes, depth + 1, maxDepth, maxBranchingFactor, i);
            }
        }

        t.Children = children;
        return t;
    }


    [Test]
    [TestCaseSource(nameof(UnaryNodeSources))]
    public void UnaryTreeShouldNotContainDuplicateEdges(IDiagramComponent[] nodes) {
        var trees = new Tree[] {
            new Tree(
                nodes.First(), nodes[1..]
            ),
            new Tree(
                nodes.Reverse().First(), nodes.Reverse().ToArray()[1..]
            )
        };

        Assert.Multiple(() => {
            foreach (Tree tree in trees) {
                var sm = tree.AsStringMap();
                var smt = sm.ToString();

                // Unary node misalignment checks
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

                // Cutoff checks
                var width = nodes
                    .Select((node) => node.AsStringMap().Width)
                    .Max();
                Assert.That(sm.Width, Is.EqualTo(width));
            }
        });
    }
}