using System.Drawing;
using System.Windows;

namespace MechEngineerSaver;

public partial class App : Application
{
    public static System.Windows.Media.Color PrimaryColor => SystemParameters.WindowGlassColor;
    public static System.Windows.Media.SolidColorBrush PrimaryBrush => new(PrimaryColor);
    public static System.Windows.Media.Color SecondaryColor => PrimaryColor.GetContrastColor();
    public static System.Windows.Media.SolidColorBrush SecondaryBrush => new(SecondaryColor);
}

public static class HSL
{
    public static System.Windows.Media.Color GetContrastColor(this System.Windows.Media.Color oldColor)
    {
        var source = Color.FromArgb(255, oldColor.R, oldColor.G, oldColor.B);

        var temp = new HSV();
        temp.h = source.GetHue();
        temp.s = source.GetSaturation();
        temp.v = getBrightness(source);
        if (temp.h > 180)
            temp.h -= 180f;
        else temp.h += 180f;

        var hslc = ColorFromHSL(temp);
        return System.Windows.Media.Color.FromArgb(hslc.A, hslc.R, hslc.G, hslc.B);
    }

    // A common triple float struct for both HSL & HSV
    // Actually this should be immutable and have a nice constructor!!
    public struct HSV { public float h; public float s; public float v; }

    // the Color Converter
    static public Color ColorFromHSL(HSV hsl)
    {
        if (hsl.s == 0)
        { int L = (int)hsl.v; return Color.FromArgb(255, L, L, L); }

        double min, max, h;
        h = hsl.h / 360d;

        max = hsl.v < 0.5d ? hsl.v * (1 + hsl.s) : (hsl.v + hsl.s) - (hsl.v * hsl.s);
        min = (hsl.v * 2d) - max;

        Color c = Color.FromArgb(255, (int)(255 * RGBChannelFromHue(min, max, h + 1 / 3d)),
                                      (int)(255 * RGBChannelFromHue(min, max, h)),
                                      (int)(255 * RGBChannelFromHue(min, max, h - 1 / 3d)));
        return c;
    }

    static double RGBChannelFromHue(double m1, double m2, double h)
    {
        h = (h + 1d) % 1d;
        if (h < 0) h += 1;
        if (h * 6 < 1) return m1 + (m2 - m1) * 6 * h;
        else if (h * 2 < 1) return m2;
        else if (h * 3 < 2) return m1 + (m2 - m1) * 6 * (2d / 3d - h);
        else return m1;
    }

    // color brightness as perceived:
    static float getBrightness(Color c) => (c.R * 0.299f + c.G * 0.587f + c.B * 0.114f) / 256f;
}
