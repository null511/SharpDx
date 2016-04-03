using SharpDX.Core.Geometry;
using SharpDX.D3DCompiler;
using SharpDX.Data.Exceptions;
using SharpDX.Direct3D11;
using System;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace SharpDX.Core.Shaders
{
    interface IShader : IDisposable
    {
        ShaderActionRegistry ActionRegistry {get;}

        void Update(DeviceContext context);
        void Apply(DeviceContext context);
    }

    static class ShaderUtils
    {
        public static CompilationResult Compile(string filename, string entryPoint, string profile) {
            var result = ShaderBytecode.CompileFromFile(filename, entryPoint, profile);
            if (result.Bytecode == null)
                throw new CompilationException(result.Message);

            return result;
        }

        public static VertexShader CompileVS(DeviceContext context, string filename, string entryPoint, string profile, IVertexDescription vertexInfo, out InputLayout layout) {
            try {
                using (var output = Compile(filename, entryPoint, profile)) {
                    var vertexShader = new VertexShader(context.Device, output);
                    layout = GetInputLayout(context, output, vertexInfo);
                    return vertexShader;
                }
            }
            catch (Exception error) {
                throw new ShaderCompilerException("Failed to compile vertex shader!", error);
            }
        }

        public static PixelShader CompilePS(DeviceContext context, string filename, string entryPoint, string profile) {
            try {
                using (var output = Compile(filename, entryPoint, profile)) {
                    return new PixelShader(context.Device, output);
                }
            }
            catch (Exception error) {
                throw new ShaderCompilerException("Failed to compile pixel shader!", error);
            }
        }

        public static InputLayout GetInputLayout(DeviceContext context, byte[] shaderByteCode, IVertexDescription vertexInfo) {
            var signature = ShaderSignature.GetInputSignature(shaderByteCode);
            return new InputLayout(context.Device, signature, vertexInfo.Elements);
        }

        public static Buffer CreateConstantBuffer<TInstance>(DeviceContext context)
            where TInstance : struct
        {
            var size = Utilities.SizeOf<TInstance>();
            return new Buffer(context.Device, size,
                ResourceUsage.Dynamic,
                BindFlags.ConstantBuffer,
                CpuAccessFlags.Write,
                ResourceOptionFlags.None,
                0);
        }

        public static void UpdateConstantBuffer<T>(DeviceContext context, Buffer buffer, ref T data)
            where T : struct
        {
            DataStream stream;
            context.MapSubresource(buffer, MapMode.WriteDiscard, MapFlags.None, out stream);
            stream.Write(data);
            context.UnmapSubresource(buffer, 0);
        }
    }
}
