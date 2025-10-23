using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace projet_wpf.Helpers
{
    internal class ColorsHelper
    {
        public static void ColorToHsv(Color color, out double h, out double s, out double v)
        {
            double r = color.R / 255.0;
            double g = color.G / 255.0;
            double b = color.B / 255.0;

            double max = Math.Max(r, Math.Max(g, b));
            double min = Math.Min(r, Math.Min(g, b));
            v = max;

            double delta = max - min;
            s = max == 0 ? 0 : delta / max;

            if (delta == 0)
            {
                h = 0;
            }
            else
            {
                if (max == r)
                    h = 60 * (((g - b) / delta) % 6);
                else if (max == g)
                    h = 60 * (((b - r) / delta) + 2);
                else // max == b
                    h = 60 * (((r - g) / delta) + 4);

                if (h < 0) h += 360;
            }
        }

        public static string GetColorCategory(Color color)
        {
            ColorToHsv(color, out double h, out double s, out double v);

            if (s < 0.18)
            {
                if (v < 0.15) return "Noir";
                if (v > 0.85) return "Blanc";
                return "Gris";
            }

            if ((h >= 345 && h <= 360) || (h >= 0 && h < 15)) return "Rouge";
            if (h >= 15 && h < 45) return "Orange";
            if (h >= 45 && h < 70) return "Jaune";
            if (h >= 70 && h < 161) return "Vert";
            if (h >= 161 && h < 200) return "Cyan";
            if (h >= 200 && h < 260) return "Bleu";
            if (h >= 260 && h < 300) return "Violet";
            if (h >= 300 && h < 345) return "Magenta";

            return "Autre";
        }

    }
}
