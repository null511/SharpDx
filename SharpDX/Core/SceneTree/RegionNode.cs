﻿using SharpDX.Core.Entities;

namespace SharpDX.Core.SceneTree
{
    class RegionNode
    {
        public RegionMap Map;
        protected BoundingBox Bounds, BoundsEx;

        private RenderCube _renderCube;
        private bool hasChildren;
        private RegionNode[] subNodes;
        private int level;
        private long? key;


        protected RegionNode(int level) {
            this.level = level;

            Bounds = new BoundingBox();
            BoundsEx = new BoundingBox();
        }

        //=============================

        protected void Create(RegionMap map) {
            this.Map = map;
        }

        protected void SetBounds(ref BoundingBox newBounds) {
            BoundsEx = Bounds = newBounds;
        }

        protected bool Insert(RenderEntity @object, int max_level) {
            var c = Bounds.Contains(ref @object.Position);
            if (c == ContainmentType.Disjoint) return false;

            if (level < max_level) {
                if (!hasChildren) createSubNodes(level+1);

                RegionNode n;
                for (int i = 0; i < 8; i++) {
                    n = subNodes[i];
                    if (n.Insert(@object, max_level)) {
                        BoundingBoxUtils.Expand(ref BoundsEx, ref n.BoundsEx);
                        return true;
                    }
                }
                return false;
            } else {
                var center = (Bounds.Minimum + Bounds.Maximum) / 2f;
                if (key == null) key = Map.GetKey(ref center);
                var cell = Map.GetOrCreateCell(key.Value);
                cell.Add(@object);

                BoundingBoxUtils.Expand(ref BoundsEx, ref @object.WorldBounds);
                return true;
            }
        }

        protected void Test(EntityCollection collection, TestOptions options) {
            ContainmentType x;
            options.Frustum.Contains(ref BoundsEx, out x);
            if (x == ContainmentType.Disjoint) return;

            if (options.EnableDebugCubeRendering &&
                level == options.DebugCubeRenderLevel)
                    options.debugCubeList.Add(RenderDebugCube());

            if (!hasChildren) {
                var cell = Map.GetCell(GetKey());
                if (cell == null) return;
                collection.AddRange(cell.OwnedObjects);
            } else {
                if (x == ContainmentType.Contains) {
                    for (int i = 0; i < 8; i++)
                        subNodes[i].AddAll(collection, options);
                } else if (x == ContainmentType.Intersects) {
                    for (int i = 0; i < 8; i++)
                        subNodes[i].Test(collection, options);
                }
            }
        }

        private RenderCube RenderDebugCube() {
            if (_renderCube == null) {
                _renderCube = new RenderCube();
                _renderCube.Color = new Color4(1f, 0f, 0f, 1f);
            }

            _renderCube.Bounds = BoundsEx;
            _renderCube.InvalidateWorld();
            return _renderCube;
        }

        protected void AddAll(EntityCollection collection, TestOptions options) {
            if (options.EnableDebugCubeRendering &&
                level == options.DebugCubeRenderLevel)
                    options.debugCubeList.Add(RenderDebugCube());

            if (hasChildren) {
                for (int i = 0; i < 8; i++)
                    subNodes[i].AddAll(collection, options);
            } else {
                var cell = Map.GetCell(GetKey());
                if (cell != null) collection.AddRange(cell.OwnedObjects);
            }
        }

        //-----------------------------

        private long GetKey() {
            if (key == null) {
                var center = (Bounds.Minimum + Bounds.Maximum) / 2f;
                key = Map.GetKey(ref center);
            }
            return key.Value;
        }

        private void createSubNodes(int level) {
            BoundsEx = new BoundingBox();
            BoundsEx.Minimum = Bounds.Minimum;
            BoundsEx.Maximum = Bounds.Maximum;

            Vector3 min, max, size;
            subNodes = new RegionNode[8];
            size = Bounds.Maximum - Bounds.Minimum;
            hasChildren = true;

            min.X = Bounds.Minimum.X;
            min.Y = Bounds.Minimum.Y + size.Y/2f;
            min.Z = Bounds.Minimum.Z + size.Z/2f;
            max.X = Bounds.Maximum.X - size.X/2f;
            max.Y = Bounds.Maximum.Y;
            max.Z = Bounds.Maximum.Z;
            createNode(Nodes.TFL, ref min, ref max, level);

            min.X = Bounds.Minimum.X + size.X/2f;
            min.Y = Bounds.Minimum.Y + size.Y/2f;
            min.Z = Bounds.Minimum.Z + size.Z/2f;
            max.X = Bounds.Maximum.X;
            max.Y = Bounds.Maximum.Y;
            max.Z = Bounds.Maximum.Z;
            createNode(Nodes.TFR, ref min, ref max, level);

            min.X = Bounds.Minimum.X;
            min.Y = Bounds.Minimum.Y + size.Y/2f;
            min.Z = Bounds.Minimum.Z;
            max.X = Bounds.Maximum.X - size.X/2f;
            max.Y = Bounds.Maximum.Y;
            max.Z = Bounds.Maximum.Z - size.Z/2f;
            createNode(Nodes.TBL, ref min, ref max, level);

            min.X = Bounds.Minimum.X + size.X/2f;
            min.Y = Bounds.Minimum.Y + size.Y/2f;
            min.Z = Bounds.Minimum.Z;
            max.X = Bounds.Maximum.X;
            max.Y = Bounds.Maximum.Y;
            max.Z = Bounds.Maximum.Z - size.Z/2f;
            createNode(Nodes.TBR, ref min, ref max, level);

            min.X = Bounds.Minimum.X;
            min.Y = Bounds.Minimum.Y;
            min.Z = Bounds.Minimum.Z + size.Z/2f;
            max.X = Bounds.Maximum.X - size.X/2f;
            max.Y = Bounds.Maximum.Y - size.Y/2f;
            max.Z = Bounds.Maximum.Z;
            createNode(Nodes.BFL, ref min, ref max, level);

            min.X = Bounds.Minimum.X + size.X/2f;
            min.Y = Bounds.Minimum.Y;
            min.Z = Bounds.Minimum.Z + size.Z/2f;
            max.X = Bounds.Maximum.X;
            max.Y = Bounds.Maximum.Y - size.Y/2f;
            max.Z = Bounds.Maximum.Z;
            createNode(Nodes.BFR, ref min, ref max, level);

            min.X = Bounds.Minimum.X;
            min.Y = Bounds.Minimum.Y;
            min.Z = Bounds.Minimum.Z;
            max.X = Bounds.Maximum.X - size.X/2f;
            max.Y = Bounds.Maximum.Y - size.Y/2f;
            max.Z = Bounds.Maximum.Z - size.Z/2f;
            createNode(Nodes.BBL, ref min, ref max, level);

            min.X = Bounds.Minimum.X + size.X/2f;
            min.Y = Bounds.Minimum.Y;
            min.Z = Bounds.Minimum.Z;
            max.X = Bounds.Maximum.X;
            max.Y = Bounds.Maximum.Y - size.Y/2f;
            max.Z = Bounds.Maximum.Z - size.Z/2f;
            createNode(Nodes.BBR, ref min, ref max, level);
        }

        private void createNode(Nodes node, ref Vector3 min, ref Vector3 max, int level) {
            var n = new RegionNode(level);
            n.BoundsEx.Minimum = n.Bounds.Minimum = min;
            n.BoundsEx.Maximum = n.Bounds.Maximum = max;
            subNodes[(int)node] = n;

            n.Create(Map);
        }

        private enum Nodes : byte {
            TFL = 0,
            TFR = 1,
            TBL = 2,
            TBR = 3,
            BFL = 4,
            BFR = 5,
            BBL = 6,
            BBR = 7,
        }
    }
}
