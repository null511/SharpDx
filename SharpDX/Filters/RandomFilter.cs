using SharpDX.Core.Filters;
using System;
using System.Threading;

namespace SharpDX.Filters
{
    class RandomFilter : IFilter
    {
        private static readonly Color4[] _cubeColors = {
            new Color4(1f, 0f, 0f, 1f),
            new Color4(0f, 1f, 0f, 1f),
            new Color4(0f, 0f, 1f, 1f),
            new Color4(1f, 1f, 0f, 1f),
            new Color4(0f, 1f, 1f, 1f),
        };

        private readonly Random _random;
        private readonly int _max;

        private int maxCubes = 100000;


        public RandomFilter(int max) {
            this._max = max;

            _random = new Random();
        }

        public bool Test(TreeBuilderTestEventArgs e) {
            if (maxCubes == 0) return false;

            var r = _random.Next(_max) == 1;
            if (r) {
                e.Color = GetRandomColor();
                Interlocked.Decrement(ref maxCubes);
            }
            return r;
        }

        private Color4 GetRandomColor() {
            var i = _random.Next(_cubeColors.Length);
            return _cubeColors[i];
        }
    }
}
