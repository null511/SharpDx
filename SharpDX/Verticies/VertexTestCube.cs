using SharpDX.Core.Geometry;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System.Runtime.InteropServices;

namespace SharpDX.Verticies
{
    [StructLayout(LayoutKind.Sequential)]
    struct VertexTestCube
    {
        public static IVertexDescription Info = new Description();
        public static IVertexDescription InstanceInfo = new InstanceDescription();

        public Vector4 Position;
        public Vector3 Normal;
        public Vector2 Texcoord;


        public VertexTestCube(Vector3 position, Vector3 normal, Vector2 texcoord) {
            this.Position = new Vector4(position, 1f);
            this.Normal = normal;
            this.Texcoord = texcoord;
        }

        public VertexTestCube(Vector4 position, Vector3 normal, Vector2 texcoord) {
            this.Position = position;
            this.Normal = normal;
            this.Texcoord = texcoord;
        }

        public override string ToString() {
            return $"X:{Position.X} Y:{Position.Y} Z:{Position.Z} W:{Position.W}"
                +$"nX:{Normal.X} nY:{Normal.Y} nZ:{Normal.Z} "
                +$"tX:{Texcoord.X} tY:{Texcoord.Y}";
        }


        public class Description : IVertexDescription
        {
            public int Size => 48;
            public InputElement[] Elements => _elements;

            private static InputElement[] _elements = new[] {
                new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                new InputElement("NORMAL", 0, Format.R32G32B32_Float, 16, 0),
                new InputElement("TEXCOORD", 0, Format.R32G32_Float, 28, 0),
            };
        }

        public class InstanceDescription : IVertexDescription
        {
            public int Size => 80;
            public InputElement[] Elements => _elements;

            private static InputElement[] _elements = new[] {
                new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0, InputClassification.PerVertexData, 0),
                new InputElement("NORMAL", 0, Format.R32G32B32_Float, 16, 0, InputClassification.PerVertexData, 0),
                new InputElement("TEXCOORD", 0, Format.R32G32_Float, 28, 0, InputClassification.PerVertexData, 0),
                new InputElement("WORLD", 0, Format.R32G32B32A32_Float, 0, 1, InputClassification.PerInstanceData, 1),
                new InputElement("WORLD", 1, Format.R32G32B32A32_Float, 16, 1, InputClassification.PerInstanceData, 1),
                new InputElement("WORLD", 2, Format.R32G32B32A32_Float, 32, 1, InputClassification.PerInstanceData, 1),
                new InputElement("WORLD", 3, Format.R32G32B32A32_Float, 48, 1, InputClassification.PerInstanceData, 1),
                new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 64, 1, InputClassification.PerInstanceData, 1),
            };
        }
    }
}
