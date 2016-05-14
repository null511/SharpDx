using SharpDX.Core.Shaders;
using SharpDX.Direct3D11;
using SharpDX.Verticies;
using System;
using System.IO;
using System.Runtime.InteropServices;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace SharpDX.Test
{
    class QuadShaderColored : IShader
    {
        private VertexShader vertexShader;
        private PixelShader pixelShader;
        private InputLayout _layout;
        private Buffer constantBuffer;
        private Vector2 _position, _size;
        private Color4 _color;
        private bool _isBufferValid;
        private bool isDisposed;
        private DataBuffer _streamBuffer;

        private ShaderActionRegistry _registry;
        public ShaderActionRegistry ActionRegistry => _registry;


        public QuadShaderColored() {
            _registry = new ShaderActionRegistry();
        }

        ~QuadShaderColored() {
            Dispose(false);
        }

        public void Dispose() {
            Dispose(true);
        }

        protected void Dispose(bool disposing) {
            if (isDisposed) return;

            _registry.Clear();
            Utilities.Dispose(ref constantBuffer);
            Utilities.Dispose(ref vertexShader);
            Utilities.Dispose(ref pixelShader);
            Utilities.Dispose(ref _layout);
            isDisposed = true;
        }

        public void Load(DeviceContext context)
        {
            var filename = Path.Combine(Environment.CurrentDirectory, "Resources\\shaders\\quad_colored.fx");

            vertexShader = ShaderUtils.CompileVS(context, filename, "VS", "vs_4_0", VertexPosition.Info, out _layout);
            pixelShader = ShaderUtils.CompilePS(context, filename, "PS", "ps_4_0");
            constantBuffer = ShaderUtils.CreateConstantBuffer<DataBuffer>(context);
        }

        public void SetPosition(ref Vector2 position) {
            _position = position;
            _isBufferValid = false;
        }

        public void SetSize(ref Vector2 size) {
            _size = size;
            _isBufferValid = false;
        }

        public void SetColor(ref Color4 color) {
            _color = color;
            _isBufferValid = false;
        }

        public void Apply(DeviceContext context) {
            context.InputAssembler.InputLayout = _layout;

            context.VertexShader.Set(vertexShader);
            context.VertexShader.SetConstantBuffer(0, constantBuffer);

            context.PixelShader.Set(pixelShader);
            context.PixelShader.SetConstantBuffer(0, constantBuffer);
        }

        public void Update(DeviceContext context) {
            if (!_isBufferValid)
                updateBuffer(context);
        }

        private void updateBuffer(DeviceContext context) {
            _streamBuffer.Position = _position;
            _streamBuffer.Size = _size;
            _streamBuffer.Color = _color;
            ShaderUtils.UpdateConstantBuffer(context, constantBuffer, ref _streamBuffer);
            _isBufferValid = true;
        }

        [StructLayout(LayoutKind.Explicit, Size = 32)]
        struct DataBuffer
        {
            [FieldOffset(0)]
            public Vector2 Position;

            [FieldOffset(8)]
            public Vector2 Size;

            [FieldOffset(16)]
            public Color4 Color;
        }
    }
}
