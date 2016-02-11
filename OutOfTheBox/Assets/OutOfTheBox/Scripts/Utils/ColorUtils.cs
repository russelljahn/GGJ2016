using UnityEngine;

namespace Assets.OutOfTheBox.Scripts.Utils
{
    public static class ColorUtils
    {
        public static Color ModifyBrightness(Color color, float amount)
        {
            color.r += amount;
            color.g += amount;
            color.b += amount;
            return color;
        }


        public static class Colors
        {
            public static Color Translucent
            {
                get { return new Color(1f, 1f, 1f, 0f); }
            }


            public static class Pinks
            {
                public static Color Pink(float alpha = 1.0f) { return new Color(1.000000f, 0.752941f, 0.796078f, alpha); }
                public static Color LightPink(float alpha = 1.0f) { return new Color(1.000000f, 0.713725f, 0.756863f, alpha); }
                public static Color HotPink(float alpha = 1.0f) { return new Color(1.000000f, 0.411765f, 0.705882f, alpha); }
                public static Color DeepPink(float alpha = 1.0f) { return new Color(1.000000f, 0.078431f, 0.576471f, alpha); }
                public static Color PaleVioletRed(float alpha = 1.0f) { return new Color(0.858824f, 0.439216f, 0.576471f, alpha); }
                public static Color MediumVioletRed(float alpha = 1.0f) { return new Color(0.780392f, 0.082353f, 0.521569f, alpha); }
            }

            public static class Reds
            {
                public static Color LightSalmon(float alpha = 1.0f) { return new Color(1.000000f, 0.627451f, 0.478431f, alpha); }
                public static Color Salmon(float alpha = 1.0f) { return new Color(0.980392f, 0.501961f, 0.447059f, alpha); }
                public static Color DarkSalmon(float alpha = 1.0f) { return new Color(0.913725f, 0.588235f, 0.478431f, alpha); }
                public static Color LightCoral(float alpha = 1.0f) { return new Color(0.941176f, 0.501961f, 0.501961f, alpha); }
                public static Color IndianRed(float alpha = 1.0f) { return new Color(0.803922f, 0.360784f, 0.360784f, alpha); }
                public static Color Crimson(float alpha = 1.0f) { return new Color(0.862745f, 0.078431f, 0.235294f, alpha); }
                public static Color FireBrick(float alpha = 1.0f) { return new Color(0.698039f, 0.133333f, 0.133333f, alpha); }
                public static Color DarkRed(float alpha = 1.0f) { return new Color(0.545098f, 0.000000f, 0.000000f, alpha); }
                public static Color Red(float alpha = 1.0f) { return new Color(1.000000f, 0.000000f, 0.000000f, alpha); }
            }

            public static class Oranges
            {
                public static Color OrangeRed(float alpha = 1.0f) { return new Color(1.000000f, 0.270588f, 0.000000f, alpha); }
                public static Color Tomato(float alpha = 1.0f) { return new Color(1.000000f, 0.388235f, 0.278431f, alpha); }
                public static Color Coral(float alpha = 1.0f) { return new Color(1.000000f, 0.498039f, 0.313725f, alpha); }
                public static Color DarkOrange(float alpha = 1.0f) { return new Color(1.000000f, 0.549020f, 0.000000f, alpha); }
                public static Color Orange(float alpha = 1.0f) { return new Color(1.000000f, 0.647059f, 0.000000f, alpha); }
            }

            public static class Yellows
            {
                public static Color Yellow(float alpha = 1.0f) { return new Color(1.000000f, 1.000000f, 0.000000f, alpha); }
                public static Color LightYellow(float alpha = 1.0f) { return new Color(1.000000f, 1.000000f, 0.878431f, alpha); }
                public static Color LemonChiffon(float alpha = 1.0f) { return new Color(1.000000f, 0.980392f, 0.803922f, alpha); }
                public static Color LightGoldenrodYellow(float alpha = 1.0f) { return new Color(0.980392f, 0.980392f, 0.823529f, alpha); }
                public static Color PapayaWhip(float alpha = 1.0f) { return new Color(1.000000f, 0.937255f, 0.835294f, alpha); }
                public static Color Moccasin(float alpha = 1.0f) { return new Color(1.000000f, 0.894118f, 0.709804f, alpha); }
                public static Color PeachPuff(float alpha = 1.0f) { return new Color(1.000000f, 0.854902f, 0.725490f, alpha); }
                public static Color PaleGoldenrod(float alpha = 1.0f) { return new Color(0.933333f, 0.909804f, 0.666667f, alpha); }
                public static Color Khaki(float alpha = 1.0f) { return new Color(0.941176f, 0.901961f, 0.549020f, alpha); }
                public static Color DarkKhaki(float alpha = 1.0f) { return new Color(0.741176f, 0.717647f, 0.419608f, alpha); }
                public static Color Gold(float alpha = 1.0f) { return new Color(1.000000f, 0.843137f, 0.000000f, alpha); }
            }

            public static class Browns
            {
                public static Color Cornsilk(float alpha = 1.0f) { return new Color(1.000000f, 0.972549f, 0.862745f, alpha); }
                public static Color BlanchedAlmond(float alpha = 1.0f) { return new Color(1.000000f, 0.921569f, 0.803922f, alpha); }
                public static Color Bisque(float alpha = 1.0f) { return new Color(1.000000f, 0.894118f, 0.768627f, alpha); }
                public static Color NavajoWhite(float alpha = 1.0f) { return new Color(1.000000f, 0.870588f, 0.678431f, alpha); }
                public static Color Wheat(float alpha = 1.0f) { return new Color(0.960784f, 0.870588f, 0.701961f, alpha); }
                public static Color BurlyWood(float alpha = 1.0f) { return new Color(0.870588f, 0.721569f, 0.529412f, alpha); }
                public static Color Tan(float alpha = 1.0f) { return new Color(0.823529f, 0.705882f, 0.549020f, alpha); }
                public static Color RosyBrown(float alpha = 1.0f) { return new Color(0.737255f, 0.560784f, 0.560784f, alpha); }
                public static Color SandyBrown(float alpha = 1.0f) { return new Color(0.956863f, 0.643137f, 0.376471f, alpha); }
                public static Color Goldenrod(float alpha = 1.0f) { return new Color(0.854902f, 0.647059f, 0.125490f, alpha); }
                public static Color DarkGoldenrod(float alpha = 1.0f) { return new Color(0.721569f, 0.525490f, 0.043137f, alpha); }
                public static Color Peru(float alpha = 1.0f) { return new Color(0.803922f, 0.521569f, 0.247059f, alpha); }
                public static Color Chocolate(float alpha = 1.0f) { return new Color(0.823529f, 0.411765f, 0.117647f, alpha); }
                public static Color SaddleBrown(float alpha = 1.0f) { return new Color(0.545098f, 0.270588f, 0.074510f, alpha); }
                public static Color Sienna(float alpha = 1.0f) { return new Color(0.627451f, 0.321569f, 0.176471f, alpha); }
                public static Color Brown(float alpha = 1.0f) { return new Color(0.647059f, 0.164706f, 0.164706f, alpha); }
                public static Color Maroon(float alpha = 1.0f) { return new Color(0.501961f, 0.000000f, 0.000000f, alpha); }
            }

            public static class Greens
            {
                public static Color DarkOliveGreen(float alpha = 1.0f) { return new Color(0.333333f, 0.419608f, 0.184314f, alpha); }
                public static Color Olive(float alpha = 1.0f) { return new Color(0.501961f, 0.501961f, 0.000000f, alpha); }
                public static Color OliveDrab(float alpha = 1.0f) { return new Color(0.419608f, 0.556863f, 0.137255f, alpha); }
                public static Color YellowGreen(float alpha = 1.0f) { return new Color(0.603922f, 0.803922f, 0.196078f, alpha); }
                public static Color LimeGreen(float alpha = 1.0f) { return new Color(0.196078f, 0.803922f, 0.196078f, alpha); }
                public static Color Lime(float alpha = 1.0f) { return new Color(0.000000f, 1.000000f, 0.000000f, alpha); }
                public static Color LawnGreen(float alpha = 1.0f) { return new Color(0.486275f, 0.988235f, 0.000000f, alpha); }
                public static Color Chartreuse(float alpha = 1.0f) { return new Color(0.498039f, 1.000000f, 0.000000f, alpha); }
                public static Color GreenYellow(float alpha = 1.0f) { return new Color(0.678431f, 1.000000f, 0.184314f, alpha); }
                public static Color SpringGreen(float alpha = 1.0f) { return new Color(0.000000f, 1.000000f, 0.498039f, alpha); }
                public static Color MediumSpringGreen(float alpha = 1.0f) { return new Color(0.000000f, 0.980392f, 0.603922f, alpha); }
                public static Color LightGreen(float alpha = 1.0f) { return new Color(0.564706f, 0.933333f, 0.564706f, alpha); }
                public static Color PaleGreen(float alpha = 1.0f) { return new Color(0.596078f, 0.984314f, 0.596078f, alpha); }
                public static Color DarkSeaGreen(float alpha = 1.0f) { return new Color(0.560784f, 0.737255f, 0.560784f, alpha); }
                public static Color MediumSeaGreen(float alpha = 1.0f) { return new Color(0.235294f, 0.701961f, 0.443137f, alpha); }
                public static Color SeaGreen(float alpha = 1.0f) { return new Color(0.180392f, 0.545098f, 0.341176f, alpha); }
                public static Color ForestGreen(float alpha = 1.0f) { return new Color(0.133333f, 0.545098f, 0.133333f, alpha); }
                public static Color Green(float alpha = 1.0f) { return new Color(0.000000f, 0.501961f, 0.000000f, alpha); }
                public static Color DarkGreen(float alpha = 1.0f) { return new Color(0.000000f, 0.392157f, 0.000000f, alpha); }
            }

            public static class Cyans
            {
                public static Color MediumAquamarine(float alpha = 1.0f) { return new Color(0.400000f, 0.803922f, 0.666667f, alpha); }
                public static Color Aqua(float alpha = 1.0f) { return new Color(0.000000f, 1.000000f, 1.000000f, alpha); }
                public static Color Cyan(float alpha = 1.0f) { return new Color(0.000000f, 1.000000f, 1.000000f, alpha); }
                public static Color LightCyan(float alpha = 1.0f) { return new Color(0.878431f, 1.000000f, 1.000000f, alpha); }
                public static Color PaleTurquoise(float alpha = 1.0f) { return new Color(0.686275f, 0.933333f, 0.933333f, alpha); }
                public static Color Aquamarine(float alpha = 1.0f) { return new Color(0.498039f, 1.000000f, 0.831373f, alpha); }
                public static Color Turquoise(float alpha = 1.0f) { return new Color(0.250980f, 0.878431f, 0.815686f, alpha); }
                public static Color MediumTurquoise(float alpha = 1.0f) { return new Color(0.282353f, 0.819608f, 0.800000f, alpha); }
                public static Color DarkTurquoise(float alpha = 1.0f) { return new Color(0.000000f, 0.807843f, 0.819608f, alpha); }
                public static Color LightSeaGreen(float alpha = 1.0f) { return new Color(0.125490f, 0.698039f, 0.666667f, alpha); }
                public static Color CadetBlue(float alpha = 1.0f) { return new Color(0.372549f, 0.619608f, 0.627451f, alpha); }
                public static Color DarkCyan(float alpha = 1.0f) { return new Color(0.000000f, 0.545098f, 0.545098f, alpha); }
                public static Color Teal(float alpha = 1.0f) { return new Color(0.000000f, 0.501961f, 0.501961f, alpha); }
            }

            public static class Blues
            {
                public static Color LightSteelBlue(float alpha = 1.0f) { return new Color(0.690196f, 0.768627f, 0.870588f, alpha); }
                public static Color PowderBlue(float alpha = 1.0f) { return new Color(0.690196f, 0.878431f, 0.901961f, alpha); }
                public static Color LightBlue(float alpha = 1.0f) { return new Color(0.678431f, 0.847059f, 0.901961f, alpha); }
                public static Color SkyBlue(float alpha = 1.0f) { return new Color(0.529412f, 0.807843f, 0.921569f, alpha); }
                public static Color LightSkyBlue(float alpha = 1.0f) { return new Color(0.529412f, 0.807843f, 0.980392f, alpha); }
                public static Color DeepSkyBlue(float alpha = 1.0f) { return new Color(0.000000f, 0.749020f, 1.000000f, alpha); }
                public static Color DodgerBlue(float alpha = 1.0f) { return new Color(0.117647f, 0.564706f, 1.000000f, alpha); }
                public static Color CornflowerBlue(float alpha = 1.0f) { return new Color(0.392157f, 0.584314f, 0.929412f, alpha); }
                public static Color SteelBlue(float alpha = 1.0f) { return new Color(0.274510f, 0.509804f, 0.705882f, alpha); }
                public static Color RoyalBlue(float alpha = 1.0f) { return new Color(0.254902f, 0.411765f, 0.882353f, alpha); }
                public static Color Blue(float alpha = 1.0f) { return new Color(0.000000f, 0.000000f, 1.000000f, alpha); }
                public static Color MediumBlue(float alpha = 1.0f) { return new Color(0.000000f, 0.000000f, 0.803922f, alpha); }
                public static Color DarkBlue(float alpha = 1.0f) { return new Color(0.000000f, 0.000000f, 0.545098f, alpha); }
                public static Color Navy(float alpha = 1.0f) { return new Color(0.000000f, 0.000000f, 0.501961f, alpha); }
                public static Color MidnightBlue(float alpha = 1.0f) { return new Color(0.098039f, 0.098039f, 0.439216f, alpha); }
            }

            public static class Purples
            {
                public static Color Lavender(float alpha = 1.0f) { return new Color(0.901961f, 0.901961f, 0.980392f, alpha); }
                public static Color Thistle(float alpha = 1.0f) { return new Color(0.847059f, 0.749020f, 0.847059f, alpha); }
                public static Color Plum(float alpha = 1.0f) { return new Color(0.866667f, 0.627451f, 0.866667f, alpha); }
                public static Color Violet(float alpha = 1.0f) { return new Color(0.933333f, 0.509804f, 0.933333f, alpha); }
                public static Color Orchid(float alpha = 1.0f) { return new Color(0.854902f, 0.439216f, 0.839216f, alpha); }
                public static Color Fuchsia(float alpha = 1.0f) { return new Color(1.000000f, 0.000000f, 1.000000f, alpha); }
                public static Color Magenta(float alpha = 1.0f) { return new Color(1.000000f, 0.000000f, 1.000000f, alpha); }
                public static Color MediumOrchid(float alpha = 1.0f) { return new Color(0.729412f, 0.333333f, 0.827451f, alpha); }
                public static Color MediumPurple(float alpha = 1.0f) { return new Color(0.576471f, 0.439216f, 0.858824f, alpha); }
                public static Color BlueViolet(float alpha = 1.0f) { return new Color(0.541176f, 0.168627f, 0.886275f, alpha); }
                public static Color DarkViolet(float alpha = 1.0f) { return new Color(0.580392f, 0.000000f, 0.827451f, alpha); }
                public static Color DarkOrchid(float alpha = 1.0f) { return new Color(0.600000f, 0.196078f, 0.800000f, alpha); }
                public static Color DarkMagenta(float alpha = 1.0f) { return new Color(0.545098f, 0.000000f, 0.545098f, alpha); }
                public static Color Purple(float alpha = 1.0f) { return new Color(0.501961f, 0.000000f, 0.501961f, alpha); }
                public static Color Indigo(float alpha = 1.0f) { return new Color(0.294118f, 0.000000f, 0.509804f, alpha); }
                public static Color DarkSlateBlue(float alpha = 1.0f) { return new Color(0.282353f, 0.239216f, 0.545098f, alpha); }
                public static Color SlateBlue(float alpha = 1.0f) { return new Color(0.415686f, 0.352941f, 0.803922f, alpha); }
                public static Color MediumSlateBlue(float alpha = 1.0f) { return new Color(0.482353f, 0.407843f, 0.933333f, alpha); }
            }

            public static class Whites
            {
                public static Color White(float alpha = 1.0f) { return new Color(1.000000f, 1.000000f, 1.000000f, alpha); }
                public static Color Snow(float alpha = 1.0f) { return new Color(1.000000f, 0.980392f, 0.980392f, alpha); }
                public static Color Honeydew(float alpha = 1.0f) { return new Color(0.941176f, 1.000000f, 0.941176f, alpha); }
                public static Color MintCream(float alpha = 1.0f) { return new Color(0.960784f, 1.000000f, 0.980392f, alpha); }
                public static Color Azure(float alpha = 1.0f) { return new Color(0.941176f, 1.000000f, 1.000000f, alpha); }
                public static Color AliceBlue(float alpha = 1.0f) { return new Color(0.941176f, 0.972549f, 1.000000f, alpha); }
                public static Color GhostWhite(float alpha = 1.0f) { return new Color(0.972549f, 0.972549f, 1.000000f, alpha); }
                public static Color WhiteSmoke(float alpha = 1.0f) { return new Color(0.960784f, 0.960784f, 0.960784f, alpha); }
                public static Color Seashell(float alpha = 1.0f) { return new Color(1.000000f, 0.960784f, 0.933333f, alpha); }
                public static Color Beige(float alpha = 1.0f) { return new Color(0.960784f, 0.960784f, 0.862745f, alpha); }
                public static Color OldLace(float alpha = 1.0f) { return new Color(0.992157f, 0.960784f, 0.901961f, alpha); }
                public static Color FloralWhite(float alpha = 1.0f) { return new Color(1.000000f, 0.980392f, 0.941176f, alpha); }
                public static Color Ivory(float alpha = 1.0f) { return new Color(1.000000f, 1.000000f, 0.941176f, alpha); }
                public static Color AntiqueWhite(float alpha = 1.0f) { return new Color(0.980392f, 0.921569f, 0.843137f, alpha); }
                public static Color Linen(float alpha = 1.0f) { return new Color(0.980392f, 0.941176f, 0.901961f, alpha); }
                public static Color LavenderBlush(float alpha = 1.0f) { return new Color(1.000000f, 0.941176f, 0.960784f, alpha); }
            }

            public static class Grays
            {
                public static Color MistyRose(float alpha = 1.0f) { return new Color(1.000000f, 0.894118f, 0.882353f, alpha); }
                public static Color Gainsboro(float alpha = 1.0f) { return new Color(0.862745f, 0.862745f, 0.862745f, alpha); }
                public static Color LightGray(float alpha = 1.0f) { return new Color(0.827451f, 0.827451f, 0.827451f, alpha); }
                public static Color Silver(float alpha = 1.0f) { return new Color(0.752941f, 0.752941f, 0.752941f, alpha); }
                public static Color DarkGray(float alpha = 1.0f) { return new Color(0.662745f, 0.662745f, 0.662745f, alpha); }
                public static Color Gray(float alpha = 1.0f) { return new Color(0.501961f, 0.501961f, 0.501961f, alpha); }
                public static Color DimGray(float alpha = 1.0f) { return new Color(0.411765f, 0.411765f, 0.411765f, alpha); }
                public static Color LightSlateGray(float alpha = 1.0f) { return new Color(0.466667f, 0.533333f, 0.600000f, alpha); }
                public static Color SlateGray(float alpha = 1.0f) { return new Color(0.439216f, 0.501961f, 0.564706f, alpha); }
                public static Color DarkSlateGray(float alpha = 1.0f) { return new Color(0.184314f, 0.309804f, 0.309804f, alpha); }
                public static Color Black(float alpha = 1.0f) { return new Color(0.000000f, 0.000000f, 0.000000f, alpha); }
            }
        }
    }
}
