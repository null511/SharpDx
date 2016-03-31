using SharpDX.Core.Entities;
using System.Collections.Generic;

namespace SharpDX.Core.SceneTree
{
    class RegionCell
    {
        private long hash;
        private IList<RenderEntity> _ownedObjects;
        private IList<RenderEntity> _intersectingObjects;

        public IList<RenderEntity> OwnedObjects => _ownedObjects;
        public IList<RenderEntity> IntersectingObjects => _intersectingObjects;


        public RegionCell(long hash) {
            this.hash = hash;

            _ownedObjects = new List<RenderEntity>();
            _intersectingObjects = new List<RenderEntity>();
        }

        public void Add(RenderEntity @object) {
            _ownedObjects.Add(@object);
        }
    }
}
