using SharpDX.Direct3D11;
using System;

namespace SharpDX.Core.Quad
{
    class QuadTextured : Quad, IDisposable
    {
        public ShaderResourceView TextureView;


        ~QuadTextured() {
            Dispose(false);
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            Utilities.Dispose(ref TextureView);
        }

        public void SetTexture(DeviceContext context, Texture2D texture) {
            Utilities.Dispose(ref TextureView);
            TextureView = new ShaderResourceView(context.Device, texture);
        }
    }
}
