using SharpDX.Core.Geometry;
using SharpDX.Core.Shaders;
using System.Collections.Generic;

namespace SharpDX.Core.Entities
{
    class ShaderMeshDictionary<TMesh, T> : ShaderDictionary<MeshDictionary<TMesh, T>>
        where TMesh : Mesh
    {
        public List<T> TryGet(IShader shader, TMesh mesh) {
            var shaderKey = this.Get(shader);
            if (shaderKey == null) return null;

            return shaderKey.Get(mesh);
        }

        public List<T> GetOrCreate(IShader shader, TMesh mesh) {
            var shaderKey = this.GetOrCreate(shader, key => new MeshDictionary<TMesh, T>());
            return shaderKey.GetOrCreate(mesh, key => new List<T>());
        }

        public MeshDictionary<TMesh, T> GetOrCreate(IShader shader) {
            return this.GetOrCreate(shader, key => new MeshDictionary<TMesh, T>());
        }
    }
}
