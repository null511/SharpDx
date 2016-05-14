using SharpDX.Core.Geometry;
using SharpDX.Core.Shaders;
using SharpDX.Direct3D11;
using System.Linq;

namespace SharpDX.Core.Entities
{
    class InstanceList : IInstanceCollection
    {
        private ShaderMeshList<InstancedMesh, InstanceData> _instances;

        public int EntityCount {get; private set;}


        public InstanceList() {
            _instances = new ShaderMeshList<InstancedMesh, InstanceData>();
        }

        public void Dispose() {
            _instances.Dispose();
        }

        public void Clear() {
            _instances.Clear();
        }

        public void ClearX() {
            foreach (var i in _instances.SelectMany(x => x.Data))
                i.Items.Clear();
        }

        public void Add(InstanceData data, IShader shader, InstancedMesh mesh) {
            _instances.GetOrCreate(shader, mesh).Add(data);
        }

        public void Add(InstanceList collection) {
            _instances.Merge(collection._instances);
        }

        public int Render(DeviceContext context) {
            IShader shader;
            int instanceCount;
            InstanceData data;
            InstancedMesh mesh;

            var renderCount = 0;
            EntityCount = 0;
            foreach (var shaderKey in _instances) {
                shader = shaderKey.Shader;
                shader.Apply(context);

                foreach (var meshKey in shaderKey.Data) {
                    mesh = meshKey.Mesh;
                    mesh.Apply(context);

                    instanceCount = meshKey.Items.Count;
                    renderCount += instanceCount;

                    for (int i = 0; i < instanceCount; i++) {
                        data = meshKey.Items[i];
                        data.Apply(context);

                        mesh.RenderInstanced(context, data.Count);
                        EntityCount += data.Count;
                    }
                }
            }

            return renderCount;
        }
    }
}
