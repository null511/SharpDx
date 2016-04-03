using SharpDX.Core.Geometry;
using SharpDX.Core.Shaders;
using SharpDX.Direct3D11;
using System.Collections.Generic;

namespace SharpDX.Core.Entities
{
    class EntityCollection : ObjectCollection<Entity>
    {
        public void Add(RenderEntity entity) {
            base.Add(entity, entity.Shader, entity.Mesh);
        }

        public void AddRange(IList<RenderEntity> entities) {
            RenderEntity e;
            for (int i = 0; i < entities.Count; i++) {
                e = entities[i];
                Add(e, e.Shader, e.Mesh);
            }
        }

        public override int Render(DeviceContext context) {
            IShader shader;
            int entityCount;
            Entity entity;
            Mesh mesh;

            var renderCount = 0;
            foreach (var shaderKey in _objects) {
                shader = shaderKey.Key;
                shader.Apply(context);

                foreach (var meshKey in shaderKey.Value) {
                    mesh = meshKey.Key;
                    mesh.Apply(context);

                    entityCount = meshKey.Value.Count;
                    for (int i = 0; i < entityCount; i++) {
                        entity = meshKey.Value[i];
                        entity.Update();

                        shader.ActionRegistry.Apply(entity);
                        shader.Update(context);

                        mesh.Render(context);
                        renderCount++;
                    }
                }
            }

            return renderCount;
        }

        public void CreateInstances(DeviceContext context, InstanceCollection collection) {
            IShader shader;
            InstancedMesh mesh;

            foreach (var shaderKey in _objects) {
                shader = shaderKey.Key;

                foreach (var meshKey in shaderKey.Value) {
                    mesh = (InstancedMesh)meshKey.Key;
                    if (mesh == null) return;

                    var data = mesh.GenerateInstances(context, meshKey.Value);
                    collection.Add(data, shader, mesh);
                }
            }
        }
    }
}
