using SharpDX.Core.Geometry;
using System.Collections.Generic;

namespace SharpDX.Core.Entities
{
    class MeshDictionary<TMesh, T> : Dictionary<TMesh, List<T>>
        where TMesh : Mesh
    {
        public List<T> GetOrCreate(TMesh mesh) {
            return this.GetOrCreate(mesh, key => new List<T>());
        }
    }
}
