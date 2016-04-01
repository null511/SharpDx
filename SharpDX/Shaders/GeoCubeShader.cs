using SharpDX.Core;
using SharpDX.Core.Geometry;
using SharpDX.Core.Shaders;
using SharpDX.Direct3D11;
using System;
using System.IO;
using System.Runtime.InteropServices;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace SharpDX.Test
{
    class GeoCubeShader : IShader
    {
        private VertexShader vertexShader;
        private PixelShader pixelShader;
        private InputLayout _layout;
        private Matrix _wv, _wvp, _wvpT;
        private Buffer constantBuffer;
        private Vector3 _sunDir;
        private bool isDisposed;
        private Color4 _color;

        private bool _isBufferValid = false;

        private ShaderActionRegistry _registry;
        public ShaderActionRegistry ActionRegistry => _registry;


        public GeoCubeShader() {
            _registry = new ShaderActionRegistry();
        }

        public void Dispose() {
            if (isDisposed) return;

            _registry.Clear();
            Utilities.Dispose(ref vertexShader);
            Utilities.Dispose(ref pixelShader);
            Utilities.Dispose(ref _layout);
            isDisposed = true;
        }

        public void Load(DeviceContext context, IVertexDescription vertexInfo)
        {
            var filename = Path.Combine(Environment.CurrentDirectory, "Resources\\shaders\\geocube.fx");

            vertexShader = ShaderUtils.CompileVS(context, filename, "VS", "vs_4_0", vertexInfo, out _layout);
            pixelShader = ShaderUtils.CompilePS(context, filename, "PS", "ps_4_0");
            constantBuffer = ShaderUtils.CreateConstantBuffer<DataBuffer>(context);
        }

        public void SetWVP(ref Matrix world, View view) {
            Matrix.Multiply(ref world, ref view.ViewMatrix, out _wv);
            Matrix.Multiply(ref _wv, ref view.ProjectionMatrix, out _wvp);
            Matrix.Transpose(ref _wvp, out _wvpT);
            _isBufferValid = false;
        }

        public void SetColor(ref Color4 color) {
            _color = color;
            _isBufferValid = false;
        }

        public void SetSunDir(ref Vector3 sunDir) {
            _sunDir = sunDir;
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
            if (!_isBufferValid) {
                updateBuffer(context);
                _isBufferValid = true;
            }
        }

        private void updateBuffer(DeviceContext context) {
            var data = new DataBuffer {
                matWVP = _wvpT,
                color = _color,
                sunDir = _sunDir,
            };

            context.UpdateSubresource(ref data, constantBuffer);
            _isBufferValid = false;
        }

        [StructLayout(LayoutKind.Explicit, Size = 112)]
        struct DataBuffer
        {
            [FieldOffset(0)]
            public Matrix matWVP;

            [FieldOffset(64)]
            public Color4 color;

            [FieldOffset(80)]
            public Vector3 sunDir;
        }
    }
}
