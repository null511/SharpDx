using SharpDX.Core.Geometry;
using SharpDX.Core.Shaders;
using SharpDX.Direct3D11;
using SharpDX.Extensions;
using System;
using System.Linq;

namespace SharpDX.Core.Entities
{
    class InstanceCollection : IDisposable
    {
        private ShaderMeshDictionary<InstancedMesh, InstanceData> _instances;

        public int EntityCount {get; private set;}


        public InstanceCollection() {
            _instances = new ShaderMeshDictionary<InstancedMesh, InstanceData>();
        }

        public void Dispose() {
            DisposeInstances();
            _instances.Clear();
        }

        public void Add(InstanceData data, IShader shader, InstancedMesh mesh) {
            _instances.GetOrCreate(shader, mesh).Add(data);
        }

        public void Add(InstanceCollection collection) {
            foreach (var shaderKey in collection._instances) {
                var meshList = collection._instances.Get(shaderKey.Key);
                var thisMeshList = _instances.GetOrCreate(shaderKey.Key);

                foreach (var meshKey in shaderKey.Value) {
                    var entityList = meshList.Get(meshKey.Key);
                    var thisEntityList = thisMeshList.GetOrCreate(meshKey.Key);

                    thisEntityList.AddRange(entityList);
                }
            }
        }

        public void Clear() {
            //foreach (var i in _instances.Values.SelectMany(x => x.Values))
            //    i.Clear();

            _instances.Clear();
        }

        public int Render(DeviceContext context) {
            IShader shader;
            int instanceCount;
            InstanceData data;
            InstancedMesh mesh;

            var renderCount = 0;
            EntityCount = 0;
            foreach (var shaderKey in _instances) {
                shader = shaderKey.Key;
                shader.Apply(context);

                foreach (var meshKey in shaderKey.Value) {
                    mesh = meshKey.Key;
                    mesh.Apply(context);

                    instanceCount = meshKey.Value.Count;
                    renderCount += instanceCount;

                    for (int i = 0; i < instanceCount; i++) {
                        data = meshKey.Value[i];
                        data.Apply(context);

                        mesh.RenderInstanced(context, data.Count);
                        EntityCount += data.Count;
                    }
                }
            }

            return renderCount;
        }

        private void DisposeInstances() {
            _instances.Values
                .SelectMany(x => x.Values)
                .SelectMany(x => x)
                .Each(x => x.Dispose());
        }
    }
}
