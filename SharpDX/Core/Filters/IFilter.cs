using SharpDX.Core.SceneTree;

namespace SharpDX.Core.Filters
{
    interface IFilter
    {
        bool Test(TreeBuilderTestEventArgs e);
    }

    class TreeBuilderTestEventArgs
    {
        public Vector3 Position;
        public TreeDescription TreeDescription;
        public int RegionX, RegionY, RegionZ;
        public int X, Y, Z;

        public Color4 Color;
    }
}
