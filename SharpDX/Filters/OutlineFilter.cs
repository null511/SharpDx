using SharpDX.Core;
using SharpDX.Core.Filters;
using SharpDX.Test;
using System.Threading;

namespace SharpDX.Filters
{
    class OutlineFilter : IFilter
    {
        private int maxCubes = 100000;


        public bool Test(TreeBuilderTestEventArgs e) {
            var max = DepthUtils.GetFaceCount(e.TreeDescription.MaxLevels) - 1;
            var r = IsBorderCell(e, max);

            if (r) {
                RandomColor.GetRGB(ref e.Color);
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
    }
}
