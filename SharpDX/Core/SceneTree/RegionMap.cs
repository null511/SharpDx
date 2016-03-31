using System;
using System.Collections.Generic;

namespace SharpDX.Core.SceneTree
{
    class RegionMap
    {
        private readonly Region _region;
        private readonly IDictionary<long, RegionCell> cells;

        private Vector3 cellSize;
        private int cellCount, cellCount2;


        public RegionMap(Region region) {
            this._region = region;

            cells = new Dictionary<long, RegionCell>();
        }

        public void Create(ref BoundingBox bounds) {
            cellCount = DepthUtils.GetFaceCount(_region.MaxLevel);
            cellSize = (bounds.Maximum - bounds.Minimum) / cellCount;
            this.cellCount2 = cellCount * cellCount;
        }

        public RegionCell Insert(Vector3 position) {
            var key = GetKey(ref position);
            return GetOrCreateCell(key);
        }

        public void Remove(long key) {
            cells.Remove(key);
        }

        public RegionCell GetCell(Vector3 position) {
            var key = GetKey(ref position);
            return GetCell(key);
        }

        public RegionCell GetCell(long key) {
            RegionCell cell;
            return cells.TryGetValue(key, out cell)
                ? cell : null;
        }

        public RegionCell GetOrCreateCell(long key) {
            var cell = GetCell(key);
            if (cell == null) {
                cell = new RegionCell(key);
                cells.Add(key, cell);
            }
            return cell;
        }

        public long GetKey(ref Vector3 position) {
            return GetKey(position.X, position.Y, position.Z);
        }

        public long GetKey(float x, float y, float z) {
            //var cubeSize = _tree.GetCubeSize();
            var min = _region.Bounds.Minimum;
            var max = _region.Bounds.Maximum;
            var _x = (x - min.X) / cellSize.X;
            var _y = (y - min.Y) / cellSize.Y;
            var _z = (z - min.Z) / cellSize.Z;
            var cx = (int)Math.Floor(_x);
            var cy = (int)Math.Floor(_y);
            var cz = (int)Math.Floor(_z);
            return cx + cy * cellCount + cz * cellCount2;
        }

        //-----------------------------

        public IEnumerable<RegionCell> Test(BoundingBox box) {
            var testCells = new Dictionary<long, RegionCell>();

            var sizeX = box.Maximum.X - box.Minimum.X;
            var sizeY = box.Maximum.Y - box.Minimum.Y;
            var sizeZ = box.Maximum.Z - box.Minimum.Z;

            var countX = Math.Ceiling(sizeX / cellSize.X);
            var countY = Math.Ceiling(sizeY / cellSize.Y);
            var countZ = Math.Ceiling(sizeZ / cellSize.Z);

            Vector3 p;
            for (int z = 0; z < countZ; z++) {
                for (int y = 0; y < countY; y++) {
                    for (int x = 0; x < countX; x++) {
                        p.X = box.Minimum.X + x * cellSize.X;
                        p.Y = box.Minimum.Y + y * cellSize.Y;
                        p.Z = box.Minimum.Z + z * cellSize.Z;

                        var key = GetKey(ref p);
                        testCells[key] = GetCell(key);
                    }
                }
            }

            return testCells.Values;
        }
    }
}
