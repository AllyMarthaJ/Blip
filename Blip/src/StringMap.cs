﻿using System.Text;
using System.Text.RegularExpressions;

namespace Blip;

public enum DrawStringMapMode {
    CROP = 0
}

public class StringMap(int width, int height) {
    public const char EMPTY_CHAR = ' ';
    private static readonly Regex SPLIT_LINE_REGEX = new(@"\r?\n", RegexOptions.Compiled);

    public int Width { get; } = width;
    public int Height { get; } = height;

    private char[] strChr = Enumerable.Repeat(EMPTY_CHAR, width * height).ToArray();

    public StringMap FillRectangle(char c, int x, int y, int width, int height) {
        ArgumentOutOfRangeException.ThrowIfNegative(width);
        ArgumentOutOfRangeException.ThrowIfNegative(height);

        int startY = Math.Clamp(y, 0, this.Height);
        int endY = Math.Clamp(y + height, 0, this.Height);

        int startX = Math.Clamp(x, 0, this.Width);
        int endX = Math.Clamp(x + width, 0, this.Width);

        int rectW = endX - startX;
        int rectH = endY - startY;

        for (int i = 0; i < rectW * rectH; i++) {
            int _x = i % rectW + startX;
            int _y = i / rectW + startY;

            this.setChar(c, _x, _y);
        }

        return this;
    }

    public StringMap DrawRectangle(char c, int x, int y, int width, int height) {
        if (x >= 0) {
            this.FillRectangle(c, x, y, 1, height);
        }

        if (y >= 0) {
            this.FillRectangle(c, x, y, width, 1);
        }

        // Offset by width of border; in this case, hardcoded to be 1.
        if (x + width - 1 < this.Width) {
            this.FillRectangle(c, x + width - 1, y, 1, height);
        }

        if (y + height - 1 < this.Height) {
            this.FillRectangle(c, x, y + height - 1, width, 1);
        }

        return this;
    }

    public char GetChar(int x, int y) {
        ArgumentOutOfRangeException.ThrowIfNegative(x);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(x, width);
        ArgumentOutOfRangeException.ThrowIfNegative(y);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(y, height);

        return this.getChar(x, y);
    }

    public StringMap SetChar(char c, int x, int y) {
        ArgumentOutOfRangeException.ThrowIfNegative(x);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(x, width);
        ArgumentOutOfRangeException.ThrowIfNegative(y);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(y, height);

        return this.setChar(c, x, y);
    }

    public StringMap DrawStringMap(StringMap sm, int x, int y) {
        return this.DrawStringMap(sm, x, y, sm.Width, sm.Height);
    }

    public StringMap DrawStringMap(StringMap sm, int x, int y, int width, int height,
        DrawStringMapMode mode = DrawStringMapMode.CROP) {
        int startY = Math.Clamp(y, 0, this.Height);
        int endY = Math.Clamp(y + height, 0, this.Height);

        int startX = Math.Clamp(x, 0, this.Width);
        int endX = Math.Clamp(x + width, 0, this.Width);

        int rectW = endX - startX;
        int rectH = endY - startY;

        switch (mode) {
            case DrawStringMapMode.CROP:
                for (int i = 0; i < rectW * rectH; i++) {
                    int originX = i % rectW;
                    int originY = i / rectW;

                    int targetX = i % rectW + startX;
                    int targetY = i / rectW + startY;

                    this.setChar(sm.getChar(originX, originY), targetX, targetY);
                }

                break;
            default:
                throw new ArgumentException(null, nameof(mode));
        }

        return this;
    }

    public static StringMap FromLineDelimitedString(string str) {
        string[] lines = SPLIT_LINE_REGEX.Split(str);
        int[] lengths = lines.Select((line) => line.Length).Distinct().ToArray();

        if (lengths.Length != 1) {
            throw new ArgumentException("Lines must all be of same length");
        }

        int width = lengths[0];
        int height = lines.Length;

        var sm = new StringMap(width, height) {
            strChr = String.Join("", lines).ToCharArray()
        };

        return sm;
    }

    public override string ToString() {
        StringBuilder sb = new();
        for (var i = 0; i < width * height; i += width) {
            sb.AppendLine(String.Join("", this.strChr[i..(i + width)]));
        }

        // Prune the newline -_-
        if (sb.Length > 1) {
            sb.Remove(sb.Length - 1, 1);
        }

        return sb.ToString();
    }
    
    private char getChar(int x, int y) {
        return this.strChr[width * y + x];
    }

    private StringMap setChar(char c, int x, int y) {
        this.strChr[width * y + x] = c;
        return this;
    }
}