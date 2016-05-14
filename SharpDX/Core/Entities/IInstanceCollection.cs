using SharpDX.Core.Geometry;
using SharpDX.Core.Shaders;
using SharpDX.Direct3D11;
using System;

namespace SharpDX.Core.Entities
{
    interface IInstanceCollection : IDisposable
    {
        int EntityCount {get;}

        void Clear();
        void Add(InstanceData data, IShader shader, InstancedMesh mesh);
        void Add(InstanceList collection);
        int Render(DeviceContext context);
    }
}
