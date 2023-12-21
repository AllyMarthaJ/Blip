namespace Blip.Diagram.Components;

public class Tree : IDiagramComponent {
    public Tree() {
        // This will nuke everything when AsStringMap is invoked.
        this.Node = this;

        this.Children = Array.Empty<IDiagramComponent>();
    }

    public Tree(IDiagramComponent node, params IDiagramComponent[] children) {
        this.Node = node;
        this.Children = children;
    }

    public int SiblingSpacing { get; set; } = 3;
    public int ParentSpacing { get; set; } = 3;

    public IDiagramComponent Node { get; set; }
    public int MaxWidth { get; set; }
    public int MaxHeight { get; set; }
    public IEnumerable<IDiagramComponent> Children { get; set; }

    public StringMap AsStringMap() {
        // Base case: This is a leaf node.
        if (!this.Children.Any()) {
            return this.Node.AsStringMap();
        }

        // Recursive step: render all children, with the top box.
        StringMap[] childrenMaps =
            this.Children
                .Select(child => child.AsStringMap())
                .ToArray();
        StringMap parentMap = this.Node.AsStringMap();

        int breadth =
            childrenMaps
                .Aggregate(0, (total, child) => total + child.Width + this.SiblingSpacing)
            - this.SiblingSpacing;
        int width = Math.Max(breadth, parentMap.Width);

        int maxHeight = childrenMaps.MaxBy(child => child.Height)!.Height;
        int childrenTop = parentMap.Height + this.ParentSpacing;

        var totalMap = new StringMap(width, childrenTop + maxHeight);

        // Draw horizontal edges.
        bool drawHorizEdge = childrenMaps.Length > 1;
        int horizEdgeTop = parentMap.Height + this.ParentSpacing / 2;

        if (drawHorizEdge) {
            int horizEdgeLeft = childrenMaps.First().Width / 2;
            int horizEdgeRight = width - childrenMaps.Last().Width / 2;

            totalMap.FillRectangle('-', horizEdgeLeft, horizEdgeTop, horizEdgeRight - horizEdgeLeft, 1);
        }

        // Parent needs a stem.
        totalMap.FillRectangle('|', (width - 1) / 2, parentMap.Height, 1, horizEdgeTop - parentMap.Height + 1);

        // McGlue those children and parent together.
        totalMap.DrawStringMap(parentMap, (width - parentMap.Width) / 2, 0);

        int left = (width - breadth) / 2;
        // Preserve parent-child alignment in the event of a dispute,
        // narrowly avoiding CPS and saving the day.
        int stemOffset = 1 - left % 2;
        foreach (StringMap childMap in childrenMaps) {
            totalMap.DrawStringMap(childMap, left, childrenTop);

            // Child needs a stem.
            totalMap.FillRectangle('|', left + (childMap.Width - stemOffset) / 2, horizEdgeTop, 1,
                childrenTop - horizEdgeTop);

            left += childMap.Width + this.SiblingSpacing;
        }

        return totalMap;
    }
}