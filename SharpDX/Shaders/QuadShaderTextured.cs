using SharpDX.Core.Shaders;
using SharpDX.Direct3D11;
using SharpDX.Verticies;
using System;
using System.IO;
using System.Runtime.InteropServices;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace SharpDX.Test
{
    using TVertex = VertexPositionTexture;

    class QuadShaderTextured : IShader
    {
        private VertexShader vertexShader;
        private PixelShader pixelShader;
        private InputLayout _layout;
        private Buffer constantBuffer;
        private Vector2 _position, _size;
        private ShaderResourceView _texture;
        private DataBuffer _streamBuffer;
        private SamplerState _sampler;
        private bool _isBufferValid;

        public ShaderActionRegistry ActionRegistry {get;}


        public QuadShaderTextured() {
            ActionRegistry = new ShaderActionRegistry();
        }

        ~QuadShaderTextured() {
            Dispose(false);
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing) {
            ActionRegistry.Clear();

            Utilities.Dispose(ref _sampler);
            Utilities.Dispose(ref constantBuffer);
            Utilities.Dispose(ref vertexShader);
            Utilities.Dispose(ref pixelShader);
            Utilities.Dispose(ref _layout);
        }

        public void Load(DeviceContext context)
        {
            var filename = Path.Combine(Environment.CurrentDirectory, "Resources\\shaders\\quad_textured.fx");

            vertexShader = ShaderUtils.CompileVS(context, filename, "VS", "vs_4_0", TVertex.Info, out _layout);
            pixelShader = ShaderUtils.CompilePS(context, filename, "PS", "ps_4_0");
            constantBuffer = ShaderUtils.CreateConstantBuffer<DataBuffer>(context);

            _sampler = new SamplerState(context.Device, new SamplerStateDescription {
                AddressU = TextureAddressMode.Wrap,
                AddressV = TextureAddressMode.Wrap,
                AddressW = TextureAddressMode.Wrap,
                Filter = Filter.MinMagMipLinear,
            });
        }

        public void SetPosition(ref Vector2 position) {
            _position = position;
            _isBufferValid = false;
        }

        public void SetSize(ref Vector2 size) {
            _size = size;
            _isBufferValid = false;
        }

        public void SetTexture(ShaderResourceView texture) {
            _texture = texture;
            _isBufferValid = false;
        }

        public void Apply(DeviceContext context) {
            context.InputAssembler.InputLayout = _layout;

            context.VertexShader.Set(vertexShader);
            context.VertexShader.SetConstantBuffer(0, constantBuffer);

            context.PixelShader.Set(pixelShader);
            context.PixelShader.SetConstantBuffer(0, constantBuffer);
            context.PixelShader.SetShaderResource(0, _texture);
            context.PixelShader.SetSampler(0, _sampler);
        }

        public void Update(DeviceContext context) {
            if (!_isBufferValid)
                updateBuffer(context);
        }

        private void updateBuffer(DeviceContext context) {
            _streamBuffer.Position = _position;
            _streamBuffer.Size = _size;
            ShaderUtils.UpdateConstantBuffer(context, constantBuffer, ref _streamBuffer);
            _isBufferValid = true;
        }

        [StructLayout(LayoutKind.Explicit, Size = 16)]
        struct DataBuffer
        {
            [FieldOffset(0)]
            public Vector2 Position;

            [FieldOffset(8)]
            public Vector2 Size;
        }
    }
}
