namespace Blip.Diagram.Components;

public class Tree(Box node, params IDiagramComponent[] children) : IDiagramComponent {
    public int SiblingSpacing { get; set; } = 3;
    public int ParentSpacing { get; set; } = 3;

    public Box Node { get; set; } = node;
    public int MaxWidth { get; set; }
    public int MaxHeight { get; set; }
    public IEnumerable<IDiagramComponent> Children { get; set; } = children;

    public StringMap AsStringMap() {
        foreach (IDiagramComponent diagramComponent in this.Children) {
            if (diagramComponent is not Box && diagramComponent is not Tree) {
                throw new ArgumentException("Unsupported node.");
            }
        }

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
        int maxHeight = childrenMaps.MaxBy(child => child.Height)!.Height;
        int childrenTop = parentMap.Height + this.ParentSpacing;

        var totalMap = new StringMap(breadth, childrenTop + maxHeight);

        // Draw horizontal edges.
        bool drawHorizEdge = childrenMaps.Length > 1;
        int horizEdgeTop = parentMap.Height + this.ParentSpacing / 2;

        if (drawHorizEdge) {
            int horizEdgeLeft = childrenMaps.First().Width / 2;
            int horizEdgeRight = breadth - childrenMaps.Last().Width / 2;

            totalMap.FillRectangle('-', horizEdgeLeft, horizEdgeTop, horizEdgeRight - horizEdgeLeft, 1);
        }

        // Parent needs a stem.
        totalMap.FillRectangle('|', breadth / 2, parentMap.Height, 1, horizEdgeTop - parentMap.Height + 1);

        // McGlue those children and parent together.
        totalMap.DrawStringMap(parentMap, (breadth - parentMap.Width) / 2, 0);

        var left = 0;
        foreach (StringMap childMap in childrenMaps) {
            totalMap.DrawStringMap(childMap, left, childrenTop);

            // Child needs a stem.
            totalMap.FillRectangle('|', left + childMap.Width / 2, horizEdgeTop, 1, childrenTop - horizEdgeTop);

            left += childMap.Width + this.SiblingSpacing;
        }

        return totalMap;
    }
}