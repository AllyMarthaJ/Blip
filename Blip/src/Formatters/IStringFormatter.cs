namespace Blip.Formatters; 

public interface IStringFormatter {
    public char[] FormatString(string str, int width, int height);
}