using SharpDX.Core;
using SharpDX.Core.Geometry;
using SharpDX.Core.Shaders;
using SharpDX.D3DCompiler;
using SharpDX.Data.Exceptions;
using SharpDX.Direct3D11;
using SharpDX.Verticies;
using System;
using System.IO;
using System.Runtime.InteropServices;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace SharpDX.Test
{
    class SceneTreeCubeShader : IShader
    {
        private VertexShader vertexShader;
        private PixelShader pixelShader;
        private InputLayout _layout;
        private Matrix _wv, _wvp, _wvpT;
        private Buffer constantBuffer;
        private bool isDisposed, _isBufferValid;
        private Color4 _color;

        private ShaderActionRegistry _registry;
        public ShaderActionRegistry ActionRegistry => _registry;


        public SceneTreeCubeShader() {
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

        private CompilationResult Compile(string filename, string entryPoint, string profile) {
            var result = ShaderBytecode.CompileFromFile(filename, entryPoint, profile);
            if (result.Bytecode == null)
                throw new CompilationException(result.Message);

            return result;
        }

        private InputLayout GetInputLayout(DeviceContext context, byte[] shaderByteCode, IVertexDescription vertexDesc) {
            var signature = ShaderSignature.GetInputSignature(shaderByteCode);
            return new InputLayout(context.Device, signature, vertexDesc.Elements);
        }

        public void Load(DeviceContext context)
        {
            var filename = Path.Combine(Environment.CurrentDirectory, "Resources\\shaders\\line.fx");

            try {
                using (var output = Compile(filename, "VS", "vs_4_0")) {
                    vertexShader = new VertexShader(context.Device, output);
                    _layout = GetInputLayout(context, output, VertexPosition.Info);
                }
            }
            catch (Exception error) {
                throw new ShaderCompilerException("Failed to compile vertex shader!", error);
            }

            try {
                using (var output = Compile(filename, "PS", "ps_4_0")) {
                    pixelShader = new PixelShader(context.Device, output);
                }
            }
            catch (Exception error) {
                throw new ShaderCompilerException("Failed to compile pixel shader!", error);
            }

            var size = Utilities.SizeOf<DataBuffer>();
            constantBuffer = new Buffer(context.Device, size,
                ResourceUsage.Default,
                BindFlags.ConstantBuffer,
                CpuAccessFlags.None,
                ResourceOptionFlags.None,
                0);
        }

        public void SetWVP(ref Matrix matWorld, View view) {
            Matrix.Multiply(ref matWorld, ref view.ViewMatrix, out _wv);
            Matrix.Multiply(ref _wv, ref view.ProjectionMatrix, out _wvp);
            Matrix.Transpose(ref _wvp, out _wvpT);
            _isBufferValid = false;
        }

        public void SetColor(ref Color4 color) {
            this._color = color;
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
            };

            context.UpdateSubresource(ref data, constantBuffer);
            _isBufferValid = false;
        }

        [StructLayout(LayoutKind.Explicit, Size = 80)]
        struct DataBuffer
        {
            [FieldOffset(0)]
            public Matrix matWVP;

            [FieldOffset(64)]
            public Color4 color;
        }
    }
}
