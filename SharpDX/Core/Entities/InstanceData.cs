using SharpDX.Direct3D11;
using System;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace SharpDX.Core.Entities
{
    class InstanceData : IDisposable
    {
        private Buffer _buffer;

        public int Count;
        public VertexBufferBinding Binding;


        public InstanceData(Buffer buffer, int stride, int count) {
            this._buffer = buffer;
            this.Count = count;

            Binding = new VertexBufferBinding(buffer, stride, 0);
        }

        public void Dispose() {
            Utilities.Dispose(ref _buffer);
        }

        public void Apply(DeviceContext context) {
            context.InputAssembler.SetVertexBuffers(1, Binding);
        }
    }
}
