using SharpDX.Core;
using SharpDX.Core.Filters;
using SharpDX.Test;
using System.Threading;

namespace SharpDX.Filters
{
    class RandomFilter : IFilter
    {
        private readonly int _chance;
        private int _max;


        public RandomFilter(int chance, int max) {
            this._chance = chance;
            this._max = max;
        }

        public bool Test(TreeBuilderTestEventArgs e) {
            if (_max == 0) return false;

            var r = RandomEx.Next(_chance) == 1;
            if (r) {
                RandomColor.GetRGB(ref e.Color);
                Interlocked.Decrement(ref _max);
            }
            return r;
        }
    }
}
