using SharpDX.Core;

namespace SharpDX.Test
{
    static class RandomColor
    {
        //private static readonly Color4[] _cubeColors = {
        //    new Color4(1f, 0f, 0f, 1f),
        //    new Color4(0f, 1f, 0f, 1f),
        //    new Color4(0f, 0f, 1f, 1f),
        //    new Color4(1f, 1f, 0f, 1f),
        //    new Color4(0f, 1f, 1f, 1f),
        //};

        public static void GetRGB(ref Color3 color) {
            color.Red = RandomEx.Next(1f);
            color.Green = RandomEx.Next(1f);
            color.Blue = RandomEx.Next(1f);
        }

        public static void GetRGB(ref Color4 color) {
            color.Red = RandomEx.Next(1f);
            color.Green = RandomEx.Next(1f);
            color.Blue = RandomEx.Next(1f);
        }

        public static void GetRGBA(ref Color4 color) {
            color.Red = RandomEx.Next(1f);
            color.Green = RandomEx.Next(1f);
            color.Blue = RandomEx.Next(1f);
            color.Alpha = RandomEx.Next(1f);
        }
    }
}
