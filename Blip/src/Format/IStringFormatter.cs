namespace Blip.Format;

public interface IStringFormatter {
    public char[] FormatString(string str, int width, int height);

    public int MeasureHeight(string str, int width);
}