using SharpDX.Direct3D11;
using System;

namespace SharpDX.Core.Shaders
{
    interface IShader : IDisposable
    {
        ShaderActionRegistry ActionRegistry {get;}

        void Update(DeviceContext context);
        void Apply(DeviceContext context);
    }
}
