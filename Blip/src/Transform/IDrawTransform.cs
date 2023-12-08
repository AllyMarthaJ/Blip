namespace Blip.Transform;

public interface IDrawTransform {
    // TODO: Allow context-specific transforms.
    public char Transform(char sourceChar, char destChar);
}