using SharpDX.Core.Geometry;
using SharpDX.Core.Shaders;
using SharpDX.Direct3D11;
using System.Collections.Generic;

namespace SharpDX.Core.Entities
{
    class EntityCollection
    {
        private readonly ShaderMeshDictionary<Mesh, IList<Entity>> entities;

        public IList<Entity> AllEntities;


        public EntityCollection() {
            AllEntities = new List<Entity>();
            entities = new ShaderMeshDictionary<Mesh, IList<Entity>>();
        }

        public bool Any() {
            return AllEntities.Count > 0;
        }

        public void Add(RenderEntity entity) {
            GetEntityList(entity.Shader, entity.Mesh).Add(entity);
            AllEntities.Add(entity);
        }

        public void AddRange(IList<RenderEntity> entities) {
            for (int i = 0; i < entities.Count; i++)
                Add(entities[i]);
        }

        public void Clear() {
            AllEntities.Clear();
            entities.Clear();
        }

        public int Render(DeviceContext context) {
            IShader shader;
            int entityCount;
            Entity entity;
            Mesh mesh;

            var renderCount = 0;
            foreach (var shaderKey in entities) {
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

        private IList<Entity> GetEntityList(IShader shader, Mesh mesh) {
            return entities.Get(shader, mesh, () => new List<Entity>());
        }

        public void CreateInstances(DeviceContext context, InstanceCollection collection) {
            IShader shader;
            InstancedMesh mesh;

            foreach (var shaderKey in entities) {
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
