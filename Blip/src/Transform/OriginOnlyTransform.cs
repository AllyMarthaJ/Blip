namespace Blip.Transform;

public class OriginOnlyTransform : IDrawTransform {
    public char Transform(char sourceChar, char destChar) {
        return sourceChar;
    }
}