using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using System;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace SharpDX.Core.Geometry
{
    class Mesh : IDisposable
    {
        protected Buffer _vertexBuffer, _indexBuffer;
        protected VertexBufferBinding _binding;
        protected int _indexCount;

        public Buffer VertexBuffer => _vertexBuffer;
        public Buffer IndexBuffer => _indexBuffer;

        public PrimitiveTopology PrimitiveTopology {get; set;}
        public DXGI.Format IndexFormat {get; set;}


        public Mesh() {
            PrimitiveTopology = PrimitiveTopology.TriangleList;
            IndexFormat = DXGI.Format.R16_UInt;
        }

        public virtual void Dispose() {
            Utilities.Dispose(ref _vertexBuffer);
            Utilities.Dispose(ref _indexBuffer);
        }

        public void Build<TVertex, TIndice>(Device device, TVertex[] vertices, TIndice[] indices, IVertexDescription vertexDesc)
            where TVertex : struct
            where TIndice : struct
        {
            _vertexBuffer = Buffer.Create(device, BindFlags.VertexBuffer, vertices);
            _indexBuffer = Buffer.Create(device, BindFlags.IndexBuffer, indices);

            _binding = new VertexBufferBinding(_vertexBuffer, vertexDesc.Size, 0);

            _indexCount = indices.Length;
        }

        public virtual void Apply(DeviceContext context) {
            context.InputAssembler.PrimitiveTopology = PrimitiveTopology;
            context.InputAssembler.SetVertexBuffers(0, _binding);
            context.InputAssembler.SetIndexBuffer(IndexBuffer, IndexFormat, 0);
        }

        public void Render(DeviceContext context) {
            context.DrawIndexed(_indexCount, 0, 0);
        }
    }
}
