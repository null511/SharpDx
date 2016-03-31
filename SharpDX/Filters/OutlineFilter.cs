using SharpDX.Core;
using SharpDX.Core.Filters;
using System;
using System.Threading;

namespace SharpDX.Filters
{
    class OutlineFilter : IFilter
    {
        private static readonly Color4[] _cubeColors = {
            new Color4(1f, 0f, 0f, 1f),
            new Color4(0f, 1f, 0f, 1f),
            new Color4(0f, 0f, 1f, 1f),
            new Color4(1f, 1f, 0f, 1f),
            new Color4(0f, 1f, 1f, 1f),
        };

        private readonly Random _random;

        private int maxCubes = 100000;


        public OutlineFilter() {
            _random = new Random();
        }

        public bool Test(TreeBuilderTestEventArgs e) {
            var max = DepthUtils.GetFaceCount(e.TreeDescription.MaxLevels) - 1;
            var r = IsBorderCell(e, max);

            if (r) {
                e.Color = GetRandomColor();
                Interlocked.Decrement(ref maxCubes);
            }
            return r;
        }

        private bool IsBorderCell(TreeBuilderTestEventArgs e, int max) {
            if (e.Y == 0 || e.Y == max) {
                if (e.X == 0 || e.X == max) return true;
                if (e.Z == 0 || e.Z == max) return true;
            }

            if ((e.X == 0 || e.X == max) && (e.Z == 0 || e.Z == max)) return true;

            return false;
        }

        private Color4 GetRandomColor() {
            var i = _random.Next(_cubeColors.Length);
            return _cubeColors[i];
        }
    }
}
