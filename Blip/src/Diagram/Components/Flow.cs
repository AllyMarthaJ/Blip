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
        int primaryAxisMaxDefault,
        Func<StringMap, int> primaryAxisMeasurement
    ) {
        List<List<StringMap>> childrenByRow = new();
        List<StringMap> currentList = new();

        var offset = 0;

        if (primaryAxisMaxDefault == 0) {
            currentList = childrenMaps.ToList();
        }
        else {
            foreach (StringMap child in childrenMaps) {
                int size = primaryAxisMeasurement(child);

                if (offset + size > primaryAxisMaxDefault) {
                    childrenByRow.Add(currentList);
                    currentList = new List<StringMap>();
                    offset = 0;
                }

                currentList.Add(child);

                // Don't worry about excluding ChildGap off the end;
                // we're not actually computing the real value, just need
                // to ensure the right things get put on to the right rows.
                offset += size + this.ChildGap;
            }
        }

        childrenByRow.Add(currentList);

        return childrenByRow;
    }

    private void drawFlowChild(StringMap sm, StringMap child, int primary, int secondary) {
        switch (this.FlowDirection) {
            case Direction.HORIZONTAL: {
                sm.DrawStringMap(child, primary, secondary);
                break;
            }
            case Direction.VERTICAL: {
                sm.DrawStringMap(child, secondary, primary);
                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public StringMap AsStringMap() {
        StringMap[] childrenMaps = this.Children
            .Select((child) => child.AsStringMap())
            .ToArray();

        // The primary axis is the one which controls the flow of 
        // children; the secondary axis provides an overflow buffer.
        var maxDefaultPrimaryAxis =
            this.FlowDirection switch {
                Direction.HORIZONTAL => this.MaxWidth,
                Direction.VERTICAL => this.MaxHeight,
                _ => throw new ArgumentOutOfRangeException()
            };
        var maxDefaultSecondaryAxis =
            this.FlowDirection switch {
                Direction.HORIZONTAL => this.MaxHeight,
                Direction.VERTICAL => this.MaxWidth,
                _ => throw new ArgumentOutOfRangeException()
            };

        Func<StringMap?, int> primaryAxisSelector =
            this.FlowDirection switch {
                Direction.HORIZONTAL => (child) => child?.Width ?? 0,
                Direction.VERTICAL => (child) => child?.Height ?? 0,
                _ => throw new ArgumentOutOfRangeException()
            };

        Func<StringMap?, int> secondaryAxisSelector =
            this.FlowDirection switch {
                Direction.HORIZONTAL => (child) => child?.Height ?? 0,
                Direction.VERTICAL => (child) => child?.Width ?? 0,
                _ => throw new ArgumentOutOfRangeException()
            };

        // Rows flow *along* the primary axis. The number of rows
        // is limited by the secondary axis maximum.
        // Rows are stacked AGAINST the secondary axis, i.e. will 
        // accumulate along it.
        var childrenByRow =
            this.splitByLine(childrenMaps, maxDefaultPrimaryAxis, primaryAxisSelector);

        // If a maximum constraint is given for the primary axis,
        // we should max use of it.
        // This allows for strict tightening of the resultant StringMap.
        var maxPrimaryAxis = maxDefaultPrimaryAxis == 0
            ? childrenByRow
                .Select((row) =>
                    row.Sum(primaryAxisSelector) +
                    Math.Max(0, row.Count - 1) * this.ChildGap)
                .Max()
            : maxDefaultPrimaryAxis;

        // A row's "size" is given by how much space it occupies on the 
        // secondary axis. 
        var rowSizes = childrenByRow
            .Select((row) => secondaryAxisSelector(row.MaxBy(secondaryAxisSelector)))
            .ToArray();

        // The number of rows, again, is limited by secondary axis.
        // The span describes the total space occupied by the rows along
        // the secondary axis, including gaps.
        var rowsToFit = 0;
        var span = 0;

        // No maximum size handed to the secondary axis means that we 
        // can overflow as much as we need to.
        if (maxDefaultSecondaryAxis == 0) {
            rowsToFit = childrenByRow.Count;
            span = rowSizes.Sum() + Math.Max(0, rowSizes.Length - 1) * this.RowGap;
        }
        else {
            // But if we limit the overflow, we should pick tbe number of rows
            // to fit. Goes without saying: this is lossy.
            for (int i = 0; i < childrenByRow.Count; i++) {
                if (span + rowSizes[i] >= maxDefaultSecondaryAxis) {
                    // Can't fit this on the next row.
                    break;
                }

                rowsToFit++;
                span += rowSizes[i];
                if (i < childrenByRow.Count - 1) {
                    span += this.RowGap;
                }
            }
        }

        // Creating the StringMap along the primary axis (width/height)
        // with the span as the secondary value allows us to fit exactly
        // the content size we need. 
        // We also guarantee span <= maxDefaultSecondaryAxis except for 0
        // maxDefaultSecondaryAxis.
        StringMap sm = this.FlowDirection switch {
            Direction.HORIZONTAL => new StringMap(maxDefaultPrimaryAxis, span),
            Direction.VERTICAL => new StringMap(span, maxDefaultPrimaryAxis),
            _ => throw new ArgumentOutOfRangeException()
        };

        // Track how far through the rows we are.
        var secondaryOffset = 0;
        
        for (int rowIdx = 0; rowIdx < rowsToFit; rowIdx++) {
            var row = childrenByRow[rowIdx];
            
            // How much space we occupy along the main axis.
            var rowSize =
                row.Sum(primaryAxisSelector) +
                Math.Max(0, row.Count - 1) * this.ChildGap;

            var primaryOffset = this.FlowAlignment switch {
                Alignment.LEFT => 0,
                Alignment.RIGHT => maxPrimaryAxis - rowSize,
                Alignment.CENTER => (maxPrimaryAxis - rowSize - 1) / 2,
                Alignment.JUSTIFY => 0,
                _ => throw new ArgumentOutOfRangeException()
            };

            if (this.FlowAlignment == Alignment.JUSTIFY) {
                if (row.Count > 1) {
                    int[] spaces = SharedHelpers.GetJustifySpaces(
                        maxPrimaryAxis,
                        // Ignore gaps: we make our own gaps for justify alignment.
                        // We use gaps to precalculate the number of items on the row,
                        // because justify alignment can't shrink; that is to say that
                        // the gap above is, by definition, the minimum gap between each item.
                        row.Sum(primaryAxisSelector),
                        row.Count
                    );

                    for (var i = 0; i < spaces.Length; i++) {
                        this.drawFlowChild(sm, row[i], primaryOffset, secondaryOffset);

                        primaryOffset += primaryAxisSelector(row[i]) + spaces[i];
                    }
                }

                // We will never draw the last gap/space, so draw it here.
                this.drawFlowChild(sm, row[^1], primaryOffset, secondaryOffset);
            }
            else {
                foreach (StringMap child in row) {
                    this.drawFlowChild(sm, child, primaryOffset, secondaryOffset);

                    primaryOffset += primaryAxisSelector(child) + this.ChildGap;
                }
            }

            secondaryOffset += rowSizes[rowIdx] + this.RowGap;
        }

        return sm;
    }
}