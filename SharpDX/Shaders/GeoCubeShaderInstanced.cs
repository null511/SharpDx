using SharpDX.Core;
using SharpDX.Core.Shaders;
using SharpDX.Direct3D11;
using SharpDX.Verticies;
using System;
using System.IO;
using System.Runtime.InteropServices;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace SharpDX.Test
{
    class GeoCubeShaderInstanced : IShader
    {
        private VertexShader vertexShader;
        private PixelShader pixelShader;
        private InputLayout _layout;
        private Matrix _vp, _vpT;
        private Buffer constantBuffer;
        private Vector3 _sunDir;
        private DataBuffer _streamBuffer;
        private DataStream _stream;
        private bool isDisposed;

        private bool _isBufferValid = false;

        private ShaderActionRegistry _registry;
        public ShaderActionRegistry ActionRegistry => _registry;


        public GeoCubeShaderInstanced() {
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

        public void Load(DeviceContext context)
        {
            var filename = Path.Combine(Environment.CurrentDirectory, "Resources\\shaders\\geocube_instanced.fx");

            vertexShader = ShaderUtils.CompileVS(context, filename, "VS", "vs_4_0", VertexTestCube.InstanceInfo, out _layout);
            pixelShader = ShaderUtils.CompilePS(context, filename, "PS", "ps_4_0");
            constantBuffer = ShaderUtils.CreateConstantBuffer<DataBuffer>(context);
        }

        public void SetVP(View view) {
            Matrix.Multiply(ref view.ViewMatrix, ref view.ProjectionMatrix, out _vp);
            Matrix.Transpose(ref _vp, out _vpT);
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
            _streamBuffer.matVP = _vpT;
            _streamBuffer.sunDir = _sunDir;
            ShaderUtils.UpdateConstantBuffer(context, constantBuffer, ref _streamBuffer);
            _isBufferValid = false;
        }

        [StructLayout(LayoutKind.Explicit, Size = 80)]
        struct DataBuffer
        {
            [FieldOffset(0)]
            public Matrix matVP;

            [FieldOffset(64)]
            public Vector3 sunDir;
        }
    }
}
