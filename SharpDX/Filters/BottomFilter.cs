using SharpDX.Core.Filters;
using SharpDX.Test;

namespace SharpDX.Filters
{
    class BottomFilter : IFilter
    {
        public bool Test(TreeBuilderTestEventArgs e) {
            var x = e.RegionY == 0 && e.Y == 0;
            if (x) RandomColor.GetRGB(ref e.Color);
            return x;
        }
    }
}
