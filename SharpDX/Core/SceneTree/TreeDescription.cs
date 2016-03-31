namespace SharpDX.Core.SceneTree
{
    class TreeDescription
    {
        public int MaxLevels;
        public int RegionCountX;
        public int RegionCountY;
        public int RegionCountZ;
        public BoundingBox Bounds;
        public Vector3 CubeSize;


        public void CalculateSize(out Vector3 treeSize) {
            Vector3 regionSize;
            Region.CalculateSize(ref CubeSize, MaxLevels, out regionSize);

            treeSize.X = regionSize.X * RegionCountX;
            treeSize.Y = regionSize.Y * RegionCountY;
            treeSize.Z = regionSize.Z * RegionCountZ;
        }
    }
}
