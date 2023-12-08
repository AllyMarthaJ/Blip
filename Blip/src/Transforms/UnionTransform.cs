namespace Blip.Transforms;

/// <summary>
///     As the idea of a union only really works over boolean logic,
///     we lose commutativity when we extend it to anything else.
///     This means that, in the event of a comparison yielding no useful
///     result, we should pick something to preserve.
/// </summary>
public enum PreservationMode {
    ORIGIN,
    DESTINATION
}

public struct UnionTransformOptions {
    public PreservationMode PreserveTarget { get; init; }
    public IEnumerable<char> Blank { get; init; }
}

public class UnionTransform(UnionTransformOptions options) : IDrawTransform {
    public char Transform(char sourceChar, char destChar) {
        bool isSourceBlank = this.isBlank(sourceChar);
        bool isDestBlank = this.isBlank(destChar);

        if (isSourceBlank && !isDestBlank) {
            return destChar;
        }

        if (!isSourceBlank && isDestBlank) {
            return sourceChar;
        }

        // If the source is also blank, we need to choose whether to use the
        // destination value, or the origin value. They could differ.
        return options.PreserveTarget switch {
            PreservationMode.ORIGIN => sourceChar,
            PreservationMode.DESTINATION => destChar,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private bool isBlank(char c) {
        return options.Blank.Contains(c);
    }
}