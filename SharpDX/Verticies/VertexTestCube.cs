using SharpDX.Core.Geometry;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System.Runtime.InteropServices;

namespace SharpDX.Verticies
{
    [StructLayout(LayoutKind.Explicit, Size = 40)]
    struct VertexTestCube
    {
        public static IVertexDescription Info = new Description();
        public static IVertexDescription InstanceInfo = new InstanceDescription();

        [FieldOffset(0)]
        public Vector4 Position;

        [FieldOffset(16)]
        public Vector3 Normal;

        [FieldOffset(32)]
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
            public int Size => 40;
            public InputElement[] Elements => _elements;

            private static InputElement[] _elements = new[] {
                new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                new InputElement("NORMAL", 0, Format.R32G32B32_Float, 16, 0),
                new InputElement("TEXCOORD", 0, Format.R32G32_Float, 32, 0),
            };
        }

        public class InstanceDescription : IVertexDescription
        {
            public int Size => 32;
            public InputElement[] Elements => _elements;

            private static InputElement[] _elements = new[] {
                new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0, InputClassification.PerVertexData, 0),
                new InputElement("NORMAL", 0, Format.R32G32B32_Float, 16, 0, InputClassification.PerVertexData, 0),
                new InputElement("TEXCOORD", 0, Format.R32G32_Float, 32, 0, InputClassification.PerVertexData, 0),
                new InputElement("POSITION", 1, Format.R32G32B32_Float, 0, 1, InputClassification.PerInstanceData, 1),
                new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 16, 1, InputClassification.PerInstanceData, 1),
            };
        }
    }

    [StructLayout(LayoutKind.Explicit, Size = 32)]
    struct InstanceTestCube
    {
        [FieldOffset(0)]
        public Vector3 Position;

        [FieldOffset(16)]
        public Color4 Color;
    }
}
