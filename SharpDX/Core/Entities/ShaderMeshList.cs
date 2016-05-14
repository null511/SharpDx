using SharpDX.Core.Geometry;
using SharpDX.Core.Shaders;
using System.Collections.Generic;

namespace SharpDX.Core.Entities
{
    class ShaderMeshList<TMesh, T> : ShaderList<MeshList<TMesh, T>>
        where TMesh : Mesh
    {
        public List<T> TryGet(IShader shader, TMesh mesh) {
            var shaderKey = this.Get(shader);
            if (shaderKey == null) return null;

            return shaderKey.Get(mesh);
        }

        public List<T> GetOrCreate(IShader shader, TMesh mesh) {
            return GetOrCreate(shader).GetOrCreate(mesh);
        }

        public MeshList<TMesh, T> GetOrCreate(IShader shader) {
            return GetOrCreate(shader, () => new MeshList<TMesh, T>());
        }

        public void Merge(ShaderMeshList<TMesh, T> collection) {
            foreach (var shaderKey in collection) {
                var meshList = collection.Get(shaderKey.Shader);
                var thisMeshList = GetOrCreate(shaderKey.Shader);

                thisMeshList.Merge(meshList);
            }
        }
    }
}
