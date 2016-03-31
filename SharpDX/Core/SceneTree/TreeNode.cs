using SharpDX.Core.Entities;
using SharpDX.Direct3D11;
using System.Collections.Generic;

namespace SharpDX.Core.SceneTree
{
    class TreeNode
    {
        public bool IsBase;
        public BoundingBox Bounds, BoundsEx;

        private Region _region;
        private List<TreeNode> _children;

        public Region Region => _region;


        public void Create(ref BoundingBox bounds, int maxLevel) {
            this.Bounds = bounds;

            if (IsBase) {
                _region = new Region(maxLevel);
                _region.Create(ref bounds);
            }
        }

        public void AddRange(IEnumerable<TreeNode> nodes) {
            if (_children == null)
                _children = new List<TreeNode>();

            _children.AddRange(nodes);
        }

        public bool Insert(RenderEntity @object) {
            var collision = Bounds.Contains(ref @object.Position);
            if (collision == ContainmentType.Disjoint) return false;

            if (IsBase) {
                if (_region.Insert(@object)) {
                    BoundingBoxUtils.Expand(ref BoundsEx, _region.BoundsEx);
                    return true;
                }
            } else {
                for (int i = 0; i < _children.Count; i++) {
                    if (_children[i].Insert(@object)) {
                        BoundingBoxUtils.Expand(ref BoundsEx, ref _children[i].BoundsEx);
                        return true;
                    }
                }
            }

            return false;
        }

        public void Test(EntityCollection collection, TestOptions options) {
            ContainmentType x;
            options.Frustum.Contains(ref BoundsEx, out x);
            if (x == ContainmentType.Disjoint) return;

            if (IsBase) {
                _region.Test(collection, options);
            } else {
                for (int i = 0; i < _children.Count; i++) {
                    if (x == ContainmentType.Contains)
                        _children[i].AddAll(collection, options);

                    if (x == ContainmentType.Intersects)
                        _children[i].Test(collection, options);
                }
            }
        }

        public void TestByRegion(IList<EntityCollection> collection, TestOptions options) {
            ContainmentType x;
            options.Frustum.Contains(ref BoundsEx, out x);
            if (x == ContainmentType.Disjoint) return;

            if (x == ContainmentType.Contains) {
                AddAllRegions(collection, options);
                return;
            }

            if (IsBase) {
                var entityCollection = new EntityCollection();
                _region.Test(entityCollection, options);

                if (entityCollection.Any())
                    collection.Add(entityCollection);
            } else {
                for (int i = 0; i < _children.Count; i++)
                    _children[i].TestByRegion(collection, options);
            }
        }

        protected void AddAll(EntityCollection collection, TestOptions options) {
            if (IsBase) {
                _region.AddAll(collection, options);
            } else {
                for (int i = 0; i < 8; i++)
                    _children[i].AddAll(collection, options);
            }
        }

        protected void AddAllRegions(IList<EntityCollection> collection, TestOptions options) {
            if (IsBase) {
                var entityCollection = new EntityCollection();
                _region.AddAll(entityCollection, options);

                if (entityCollection.Any())
                collection.Add(entityCollection);
            } else {
                for (int i = 0; i < 8; i++)
                    _children[i].AddAllRegions(collection, options);
            }
        }
    }
}
