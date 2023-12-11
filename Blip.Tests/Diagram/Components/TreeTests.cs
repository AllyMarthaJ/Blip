using Blip.Diagram.Components;
using Blip.Format;

namespace Blip.Tests.Diagram.Components;

public class TreeTests {
    private static object[] UnaryNodeSources = {
        new object[] { new[] { new Text(Alignment.LEFT, "pandas"), new Text(Alignment.LEFT, "aaa") } },
        new object[] { new[] { new Text(Alignment.LEFT, "pandas"), new Text(Alignment.LEFT, "aa") } },
        new object[] { new[] { new Text(Alignment.LEFT, "pandas"), new Text(Alignment.LEFT, "a") } },
        new object[] { new[] { new Text(Alignment.LEFT, "panda"), new Text(Alignment.LEFT, "aaa") } },
        new object[] { new[] { new Text(Alignment.LEFT, "panda"), new Text(Alignment.LEFT, "aa") } },
        new object[] { new[] { new Text(Alignment.LEFT, "panda"), new Text(Alignment.LEFT, "a") } },
        new object[] { new[] { new Text(Alignment.LEFT, "aaa"), new Text(Alignment.LEFT, "aaa") } },
        new object[] { new[] { new Text(Alignment.LEFT, "aaaa"), new Text(Alignment.LEFT, "aaaa") } },
        // Technically not necessary: these tests test symmetry and 
        // tuple alignment; trees are just supersets of subtrees and
        // so on until you get to pairs...then pairs of pairs...
        // and so on...
        new object[] {
            new[] {
                new Text(Alignment.LEFT, "pandas"), new Text(Alignment.LEFT, "a"), new Text(Alignment.LEFT, "aaa"),
                new Text(Alignment.LEFT, "panda"), new Text(Alignment.LEFT, "pandas")
            }
        }
    };

    [Test]
    public void FuzzTestTrees() {
        var testCount = 10;

        var rnd = new Random();
        for (var i = 0; i < testCount; i++) {
            var availableNodes = 100;
            var maxNodes = 100;

            StringMap treeSf =
                this.generateRandomTree(
                        rnd,
                        ref availableNodes,
                        maxBranchingFactor: 3,
                        maxDepth: 4
                    )
                    .AsStringMap();

            int usedNodes = maxNodes - availableNodes;

            Assert.That(treeSf.ToString(), Has.Exactly(usedNodes).EqualTo('a'));
        }
    }

    private Tree generateRandomTree(
        Random rnd,
        ref int maxNodes,
        int depth = 1,
        int maxDepth = 3,
        int maxBranchingFactor = 3,
        int idx = 0
    ) {
        var t = new Tree(new Text(Alignment.LEFT, "a"))
            { SiblingSpacing = 2, ParentSpacing = 1 };
        maxNodes -= 1;

        if (maxNodes <= 1 || depth == maxDepth) {
            return t;
        }

        // How many children should this tree have?
        Tree[] children = new Tree[Math.Min(rnd.Next(maxNodes), maxBranchingFactor)];

        bool reverse = rnd.Next(2) == 0;
        if (reverse) {
            for (int i = children.Length - 1; i >= 0; i--) {
                children[i] = this.generateRandomTree(rnd, ref maxNodes, depth + 1, maxDepth, maxBranchingFactor, i);
            }
        }
        else {
            for (var i = 0; i < children.Length; i++) {
                children[i] = this.generateRandomTree(rnd, ref maxNodes, depth + 1, maxDepth, maxBranchingFactor, i);
            }
        }

        t.Children = children;
        return t;
    }

    private Tree generateUnaryTree(IDiagramComponent[] nodes) {
        return new Tree(
            nodes.First(),
            nodes.Length > 1
                ? new IDiagramComponent[] { this.generateUnaryTree(nodes[1..]) }
                : Array.Empty<IDiagramComponent>()
        );
    }

    [Test]
    [TestCaseSource(nameof(UnaryNodeSources))]
    public void UnaryTreeShouldNotContainDuplicateEdges(IDiagramComponent[] nodes) {
        Tree[] trees = {
            this.generateUnaryTree(nodes), this.generateUnaryTree(nodes.Reverse().ToArray())
        };

        Assert.Multiple(() => {
            foreach (Tree tree in trees) {
                StringMap sm = tree.AsStringMap();
                var smt = sm.ToString();

                // Unary node misalignment checks
                Assert.That(smt, Does.Not.Contain("||"));
                Assert.That(smt, Does.Not.Contain("| |"));

                for (var y = 0; y < sm.Height; y++) {
                    for (var x = 0; x < sm.Width; x++) {
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
                int width = nodes
                    .Select(node => node.AsStringMap().Width)
                    .Max();
                Assert.That(sm.Width, Is.EqualTo(width));
            }
        });
    }
}