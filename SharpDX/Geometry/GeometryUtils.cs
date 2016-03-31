using System;

namespace SharpDX.Geometry
{
    static class GeometryUtils
    {
        public static void AssertPoints<T>(ref T[] points, string argumentName, int size) {
            if (points == null)
                throw new ArgumentNullException(argumentName);

            if (points.Length != size)
                throw new ArgumentOutOfRangeException(argumentName, points.Length, $"Expected {size} points!");
        }
    }
}
