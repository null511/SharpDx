using SharpDX.Core;
using SharpDX.Core.Filters;
using SharpDX.Test;
using System.Threading;

namespace SharpDX.Filters
{
    class OutlineAllFilter : IFilter
    {
        public bool Test(TreeBuilderTestEventArgs e) {
            var max = DepthUtils.GetFaceCount(e.TreeDescription.MaxLevels) - 1;
            var r = IsBorderCell(e, max);

            if (r) RandomColor.GetRGB(ref e.Color);
            return r;
        }

        private bool IsBorderCell(TreeBuilderTestEventArgs e, int max) {
            var isBottom = e.Y == 0 && e.RegionY == 0;
            var isTop = e.Y == max && e.RegionY == e.TreeDescription.RegionCountY - 1;

            var isFront = e.X == 0 && e.RegionX == 0;
            var isBack = e.X == max && e.RegionX == e.TreeDescription.RegionCountX - 1;

            var isLeft = e.Z == 0 && e.RegionZ == 0;
            var isRight = e.Z == max && e.RegionZ == e.TreeDescription.RegionCountZ - 1;

            if (isBottom || isTop) {
                if (isFront || isBack) return true;
                if (isLeft || isRight) return true;
            }

            if ((isFront || isBack) && (isLeft || isRight)) return true;

            return false;
        }
    }
}
