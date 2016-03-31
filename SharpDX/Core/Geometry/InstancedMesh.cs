using SharpDX.Core.Entities;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace SharpDX.Core.Geometry
{
    delegate void BuildEvent<TEntity, TInstance>(TEntity entity, ref TInstance data)
        where TEntity : Entity
        where TInstance : struct;

    class InstancedMesh : Mesh
    {
        private Func<DeviceContext, IList<Entity>, InstanceData> _buildEvent;


        public void OnBuildInstance<TEntity, TInstance>(BuildEvent<TEntity, TInstance> instanceBuildEvent)
            where TEntity : Entity
            where TInstance : struct
        {
            _buildEvent = (context, entities) => Build(context, entities, instanceBuildEvent);
        }

        private InstanceData Build<TEntity, TInstance>(DeviceContext context, IList<Entity> entities, BuildEvent<TEntity, TInstance> instanceBuildEvent)
            where TEntity : Entity
            where TInstance : struct
        {
            if (instanceBuildEvent == null)
                throw new ApplicationException("No InstanceBuildEvent specified!");

            TEntity e;
            var entityCount = entities.Count;
            var data = new TInstance[entityCount];
            for (int i = 0; i < entityCount; i++) {
                e = (TEntity)entities[i];
                if (e == null) continue;

                e.Update();

                instanceBuildEvent(e, ref data[i]);
            }

            var itemSize = Utilities.SizeOf<TInstance>();
            var bufferSize = itemSize * entityCount;
            var buffer = Buffer.Create(context.Device, BindFlags.VertexBuffer, data, bufferSize, ResourceUsage.Dynamic, CpuAccessFlags.Write, ResourceOptionFlags.None, itemSize);

            return new InstanceData(buffer, itemSize, entityCount);
        }

        public InstanceData GenerateInstances(DeviceContext context, IList<Entity> entities) {
            return _buildEvent(context, entities);
        }

        public void RenderInstanced(DeviceContext context, int instanceCount) {
            context.DrawIndexedInstanced(_indexCount, instanceCount, 0, 0, 0);
        }
    }
}
