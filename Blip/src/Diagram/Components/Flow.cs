using Blip.Diagram.Styles;
using Blip.Format;

namespace Blip.Diagram.Components;

public class Flow : IDiagramComponent {
    public int MaxWidth { get; set; }
    public int MaxHeight { get; set; }
    public IEnumerable<IDiagramComponent> Children { get; set; }

    public int ChildGap { get; set; } = 1;

    public int RowGap { get; set; } = 1;

    public Alignment FlowAlignment { get; set; } = Alignment.LEFT;

    public Direction FlowDirection { get; set; } = Direction.HORIZONTAL;

    private List<List<StringMap>> splitByLine(
        StringMap[] childrenMaps,
        int maxPrimaryAxis,
        Func<StringMap, int> primaryAxisMeasurement
    ) {
        List<List<StringMap>> childrenByRow = new();
        List<StringMap> currentList = new();

        var left = 0;

        if (maxPrimaryAxis == 0) {
            currentList = childrenMaps.ToList();
        }
        else {
            foreach (StringMap t in childrenMaps) {
                var size = primaryAxisMeasurement(t);

                if (left + size >= maxPrimaryAxis) {
                    childrenByRow.Add(currentList);
                    currentList = new List<StringMap>();
                    left = 0;
                }

                currentList.Add(t);

                // Don't worry about excluding ChildGap off the end;
                // we're not actually computing the real value, just need
                // to ensure the right things get put on to the right rows.
                left += size + this.ChildGap;
            }
        }

        childrenByRow.Add(currentList);

        return childrenByRow;
    }

    public StringMap AsStringMap() {
        var childrenMaps = this.Children
            .Select((child) => child.AsStringMap())
            .ToArray();
        var childrenByRow =
            this.splitByLine(childrenMaps, this.MaxWidth, (s) => s.Width);

        // If MaxWidth given, we should use that.
        var maxWidth = this.MaxWidth == 0
            ? childrenByRow
                .Select((row) =>
                    row.Sum((child) => child.Width) +
                    Math.Max(0, row.Count - 1) * this.ChildGap)
                .Max()
            : this.MaxWidth;

        var rowHeights = childrenByRow
            .Select((row) => row.MaxBy((child) => child.Height)?.Height ?? 0)
            .ToArray();

        var rowsToFit = 0;
        var height = 0;

        if (this.MaxHeight == 0) {
            rowsToFit = childrenByRow.Count;
            height = rowHeights.Sum() + Math.Max(0, rowHeights.Length - 1) * this.RowGap;
        }
        else {
            for (int i = 0; i < childrenByRow.Count; i++) {
                if (height + rowHeights[i] >= this.MaxHeight) {
                    // Can't fit this on the next row.
                    break;
                }

                rowsToFit++;
                height += rowHeights[i];
                if (i < childrenByRow.Count - 1) {
                    height += this.RowGap;
                }
            }
        }

        StringMap sm = new(maxWidth, height);

        var top = 0;
        for (int rowIdx = 0; rowIdx < rowsToFit; rowIdx++) {
            var row = childrenByRow[rowIdx];
            var rowWidth =
                row.Sum((child) => child.Width) +
                Math.Max(0, row.Count - 1) * this.ChildGap;

            var left = this.FlowAlignment switch {
                Alignment.LEFT => 0,
                Alignment.RIGHT => maxWidth - rowWidth,
                Alignment.CENTER => (maxWidth - rowWidth - 1) / 2,
                Alignment.JUSTIFY => 0,
                _ => throw new ArgumentOutOfRangeException()
            };

            if (this.FlowAlignment == Alignment.JUSTIFY) {
                if (row.Count > 1) {
                    int[] spaces = SharedHelpers.GetJustifySpaces(
                        maxWidth,
                        row.Sum(child => child.Width),
                        row.Count
                    );

                    for (int i = 0; i < spaces.Length; i++) {
                        sm.DrawStringMap(row[i], left, top);

                        left += row[i].Width + spaces[i];
                    }
                }

                sm.DrawStringMap(row[^1], left, top);
            }
            else {
                foreach (StringMap t in row) {
                    sm.DrawStringMap(t, left, top);

                    left += t.Width + this.ChildGap;
                }
            }

            top += rowHeights[rowIdx] + this.RowGap;
        }

        return sm;
    }
}