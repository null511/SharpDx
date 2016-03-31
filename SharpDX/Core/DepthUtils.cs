using System;

namespace SharpDX.Core
{
    static class DepthUtils
    {
        public static int GetFaceCount(int depth) {
            return (int)Math.Pow(2, depth - 1);
        }

        public static void GetFaceCount(int depth, out long value) {
            value = (long)Math.Pow(2, depth - 1);
        }

        public static int GetVertexCount(int depth) {
            return (int)Math.Pow(2, depth - 1) + 1;
        }

        public static void GetVertexCount(int depth, out long value) {
            value = (long)Math.Pow(2, depth - 1) + 1;
        }
    }
}
