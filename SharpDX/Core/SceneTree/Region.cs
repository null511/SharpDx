using SharpDX.Core.Entities;
using System;

namespace SharpDX.Core.SceneTree
{
    class Region : RegionNode
    {
        private readonly RegionMap map;
        private readonly int _maxLevel;

        public new BoundingBox Bounds => base.Bounds;
        public new BoundingBox BoundsEx => base.BoundsEx;
        public int MaxLevel => _maxLevel;


        public Region(int maxLevel) : base(1) {
            this._maxLevel = maxLevel;

            map = new RegionMap(this);
        }

        public void Create(ref BoundingBox bounds) {
            map.Create(ref bounds);
            base.SetBounds(ref bounds);
            base.Create(map);
        }

        public bool Insert(RenderEntity @object) {
            return base.Insert(@object, _maxLevel);
        }

        public static Vector3 GetCubeSize(int level, ref BoundingBox bounds) {
            var cubeCount = DepthUtils.GetFaceCount(level);
            return (bounds.Maximum - bounds.Minimum) / cubeCount;
        }

        public Vector3 GetCubeSize(int level) {
            return GetCubeSize(level, ref base.Bounds);
        }

        public Vector3 GetCubeSize() {
            return GetCubeSize(_maxLevel, ref base.Bounds);
        }

        public void SnapToGrid(ref Vector3 position, int level) {
            var cubeSize = GetCubeSize(level);

            var _x = (position.X - base.Bounds.Minimum.X) / cubeSize.X;
            var _y = (position.Y - base.Bounds.Minimum.Y) / cubeSize.Y;
            var _z = (position.Z - base.Bounds.Minimum.Z) / cubeSize.Z;

            var _fx = (int)Math.Floor(_x);
            var _fy = (int)Math.Floor(_y);
            var _fz = (int)Math.Floor(_z);

            position.X = base.Bounds.Minimum.X + _fx * cubeSize.X;
            position.Y = base.Bounds.Minimum.Y + _fy * cubeSize.Y;
            position.Z = base.Bounds.Minimum.Z + _fz * cubeSize.Z;
        }

        public new void Test(EntityCollection collection, TestOptions options) {
            base.Test(collection, options);
        }

        public static void CalculateSize(ref Vector3 cubeSize, int maxLevel, out Vector3 regionSize) {
            var cubeCount = DepthUtils.GetFaceCount(maxLevel);
            regionSize.X = cubeSize.X * cubeCount;
            regionSize.Y = cubeSize.Y * cubeCount;
            regionSize.Z = cubeSize.Z * cubeCount;
        }

        public new void AddAll(EntityCollection collection, TestOptions options) {
            base.AddAll(collection, options);
        }
    }
}
