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

        public void ClearEntities() {
            foreach (var shaderKey in _objects) {
                foreach (var meshKey in shaderKey.Value) {
                    meshKey.Value.Clear();
                }
            }
        }

        public override int Render(DeviceContext context) {
            IShader shader;
            int entityCount;
            Entity entity;
            Mesh mesh;

            var isShaderApplied = false;
            var isMeshApplied = false;

            var renderCount = 0;
            foreach (var shaderKey in _objects) {
                shader = shaderKey.Key;
                isShaderApplied = false;

                foreach (var meshKey in shaderKey.Value) {
                    mesh = meshKey.Key;
                    isMeshApplied = false;

                    entityCount = meshKey.Value.Count;
                    for (int i = 0; i < entityCount; i++) {
                        if (!isShaderApplied) {
                            shader.Apply(context);
                            isShaderApplied = true;
                        }

                        if (!isMeshApplied) {
                            mesh.Apply(context);
                            isMeshApplied = true;
                        }

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

        public void CreateInstances(DeviceContext context, IInstanceCollection collection) {
            IShader shader;
            InstancedMesh mesh;

            foreach (var shaderKey in _objects) {
                shader = shaderKey.Key;

                foreach (var meshKey in shaderKey.Value) {
                    mesh = (InstancedMesh)meshKey.Key;

                    var data = mesh.GenerateInstances(context, meshKey.Value);
                    collection.Add(data, shader, mesh);
                }
            }
        }
    }
}
