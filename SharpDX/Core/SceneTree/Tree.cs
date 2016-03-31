using SharpDX.Core.Entities;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;

namespace SharpDX.Core.SceneTree
{
    class Tree
    {
        public readonly TreeDescription Description;
        public BoundingBox BoundsEx;

        private readonly NodeArray<TreeNode> _regions;
        private TreeNode _root;
        private Vector3 _regionSize;



        public Tree(TreeDescription description) {
            this.Description = description;

            _regions = new NodeArray<TreeNode>(description.RegionCountX, description.RegionCountY, description.RegionCountZ);
        }

        public void Create() {
            Vector3 size;
            Vector3.Subtract(ref Description.Bounds.Maximum, ref Description.Bounds.Minimum, out size);

            _regionSize.X = size.X / Description.RegionCountX;
            _regionSize.Y = size.Y / Description.RegionCountY;
            _regionSize.Z = size.Z / Description.RegionCountZ;

            CreateBaseRegions();
            _root = BuildTree(_regions);
        }

        private TreeNode BuildTree(NodeArray<TreeNode> regions) {
            var newSizeX = (int)Math.Ceiling(regions.SizeX / 2f);
            var newSizeY = (int)Math.Ceiling(regions.SizeY / 2f);
            var newSizeZ = (int)Math.Ceiling(regions.SizeZ / 2f);

            var result = new NodeArray<TreeNode>(newSizeX, newSizeY, newSizeZ);

            BoundingBox bounds;
            TreeNode n;
            for (int z = 0; z < regions.SizeZ; z += 2) {
                for (int y = 0; y < regions.SizeY; y += 2) {
                    for (int x = 0; x < regions.SizeX; x += 2) {
                        n = new TreeNode();
                        var children = GetSubRegion(regions, x, y, z);
                        n.AddRange(children);

                        var isFirst = true;
                        bounds = new BoundingBox();
                        foreach (var i in children) {
                            if (isFirst) {
                                bounds = i.Bounds;
                                isFirst = false;
                            } else
                                BoundingBoxUtils.Expand(ref bounds, ref i.Bounds);
                        }

                        n.Create(ref bounds, Description.MaxLevels);

                        result.Set(x/2, y/2, z/2, n);
                    }
                }
            }

            if (newSizeX == 1 && newSizeY == 1 && newSizeZ == 1) {
                return result.Get(0, 0, 0);
            }

            return BuildTree(result);
        }

        private IEnumerable<TreeNode> GetSubRegion(NodeArray<TreeNode> regions, int x, int y, int z) {
            TreeNode n;
            for (var iz = 0; iz < 2; iz++) {
                for (var iy = 0; iy < 2; iy++) {
                    for (var ix = 0; ix < 2; ix++) {
                        if (x + ix >= regions.SizeX) continue;
                        if (y + iy >= regions.SizeY) continue;
                        if (z + iz >= regions.SizeZ) continue;

                        n = regions.Get(x + ix, y + iy, z + iz);
                        if (n != null) yield return n;
                    }
                }
            }
        }

        public bool Insert(RenderEntity @object) {
            return _root.Insert(@object);
        }

        private void CreateBaseRegions() {
            var min = Description.Bounds.Minimum;

            Vector3 p;
            BoundingBox worldBounds;
            TreeNode n;
            for (int z = 0; z < Description.RegionCountZ; z++) {
                for (int y = 0; y < Description.RegionCountY; y++) {
                    for (int x = 0; x < Description.RegionCountX; x++) {
                        p.X = min.X + x * _regionSize.X;
                        p.Y = min.Y + y * _regionSize.Y;
                        p.Z = min.Z + z * _regionSize.Z;

                        worldBounds.Minimum = p;
                        worldBounds.Maximum = p + _regionSize;

                        n = new TreeNode();
                        n.IsBase = true;
                        n.Create(ref worldBounds, Description.MaxLevels);

                        _regions.Set(x, y, z, n);
                    }
                }
            }
        }

        public void SnapToGrid(ref Vector3 position) {
            SnapToGrid(ref position, Description.MaxLevels);
        }

        public void SnapToGrid(ref Vector3 position, int level) {
            var x = (position.X - Description.Bounds.Minimum.X) / _regionSize.X;
            var y = (position.Y - Description.Bounds.Minimum.Y) / _regionSize.Y;
            var z = (position.Z - Description.Bounds.Minimum.Z) / _regionSize.Z;

            var fx = (int)Math.Floor(x);
            var fy = (int)Math.Floor(y);
            var fz = (int)Math.Floor(z);

            _regions.Get(fx, fy, fz).Region.SnapToGrid(ref position, level);
        }

        public void Test(EntityCollection collection, TestOptions options) {
            collection.Clear();
            options.debugCubeList?.Clear();
            _root.Test(collection, options);
        }

        public void TestByRegion(DeviceContext context, IList<EntityCollection> collection, TestOptions options) {
            collection.Clear();
            options.debugCubeList?.Clear();
            _root.TestByRegion(collection, options);
        }

        public Region GetRegion(int x, int y, int z) {
            return _regions.Get(x, y, z).Region;
        }
    }
}
