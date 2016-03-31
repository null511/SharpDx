using SharpDX.Core.Geometry;
using SharpDX.Core.Shaders;
using SharpDX.Direct3D11;
using SharpDX.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpDX.Core.Entities
{
    class InstanceCollection : IDisposable
    {
        private ShaderMeshDictionary<InstancedMesh, IList<InstanceData>> _instances;


        public InstanceCollection() {
            _instances = new ShaderMeshDictionary<InstancedMesh, IList<InstanceData>>();
        }

        public void Dispose() {
            DisposeInstances();
        }

        public void Add(InstanceData data, IShader shader, InstancedMesh mesh) {
            GetEntityList(shader, mesh).Add(data);
        }

        public void Clear() {
            DisposeInstances();
            _instances.Clear();
        }

        public int Render(DeviceContext context) {
            IShader shader;
            int instanceCount;
            InstanceData data;
            InstancedMesh mesh;

            var renderCount = 0;
            foreach (var shaderKey in _instances) {
                shader = shaderKey.Key;
                shader.Apply(context);

                foreach (var meshKey in shaderKey.Value) {
                    mesh = meshKey.Key;
                    mesh.Apply(context);

                    instanceCount = meshKey.Value.Count;
                    for (int i = 0; i < instanceCount; i++) {
                        data = meshKey.Value[i];

                        context.InputAssembler.SetVertexBuffers(1, data.Binding);

                        data.Apply(context);

                        mesh.RenderInstanced(context, data.Count);
                        renderCount += data.Count;
                    }
                }
            }

            return renderCount;
        }

        private IList<InstanceData> GetEntityList(IShader shader, InstancedMesh mesh) {
            var meshList = _instances.Get(shader, () => new Dictionary<InstancedMesh, IList<InstanceData>>());

            IList<InstanceData> instanceData;
            if (meshList.TryGetValue(mesh, out instanceData))
                return instanceData;

            instanceData = new List<InstanceData>();
            meshList.Add(mesh, instanceData);
            return instanceData;
        }

        private void DisposeInstances() {
            _instances.Values
                .SelectMany(x => x.Values)
                .SelectMany(x => x)
                .Each(x => x.Dispose());
        }
    }
}
