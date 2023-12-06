namespace Blip.Formatters; 

public interface IStringFormatter {
    public string[] FormatString(string str, int width, int height);
}