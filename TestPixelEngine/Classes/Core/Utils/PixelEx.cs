using PixelEngine;
using UnityEngine;

namespace EvoGenome
{
    public static class PixelEx
    {
        public static Pixel ColorToPixel(this Color c)
        {
            return new Pixel(
                (byte)(c.r * 255), 
                (byte)(c.g * 255), 
                (byte)(c.b * 255), 
                (byte)(c.a * 255));
        }

        public static Pixel Lerp(Pixel a, Pixel b, float t)
        {
            t = Mathf.Clamp01(t);
            return new Pixel(
                (byte)(a.R + (b.R - (float)a.R) * t),
                (byte)(a.G + (b.G - (float)a.G) * t),
                (byte)(a.B + (b.B - (float)a.B) * t),
                (byte)(a.A + (b.A - (float)a.A) * t));
        }

        public static Pixel CombinedColors(bool averageColor, params Pixel[] pixels)
        {
            float r = 0, g = 0, b = 0, a = 0;

            foreach (var pix in pixels)
            {
                r += pix.R;
                g += pix.G;
                b += pix.B;
                a += pix.A;
            }

            if (averageColor)
            {
                r /= pixels.Length;
                g /= pixels.Length;
                b /= pixels.Length;
                a /= pixels.Length;
            }

            return new Pixel((byte)r, (byte)g, (byte)b, (byte)a);
        }

        public static Pixel Multiply(this Pixel pix, float clamp01)
        {
            clamp01 = Mathf.Clamp01(clamp01);
            return new Pixel(
                (byte)(pix.R * clamp01),
                (byte)(pix.G * clamp01),
                (byte)(pix.B * clamp01),
                (byte)(pix.A * clamp01));
        }
    }
}
