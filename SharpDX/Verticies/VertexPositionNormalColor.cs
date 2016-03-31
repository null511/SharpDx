using SharpDX.Core.Geometry;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System.Runtime.InteropServices;

namespace SharpDX.Verticies
{
    [StructLayout(LayoutKind.Sequential)]
    struct VertexPositionNormalColor
    {
        public static IVertexDescription Info = new Description();

        public Vector4 Position;
        public Vector3 Normal;
        public Color4 Color;


        public VertexPositionNormalColor(Vector3 position, Vector3 normal, Color4 color) {
            this.Position = new Vector4(position, 1f);
            this.Normal = normal;
            this.Color = color;
        }

        public VertexPositionNormalColor(Vector4 position, Vector3 normal, Color4 color) {
            this.Position = position;
            this.Normal = normal;
            this.Color = color;
        }

        public override string ToString() {
            return $"X:{Position.X} Y:{Position.Y} Z:{Position.Z} W:{Position.W}"
                +$"nX:{Normal.X} nY:{Normal.Y} nZ:{Normal.Z} "
                +$"R:{Color.Red} G:{Color.Green} B:{Color.Blue} A:{Color.Alpha}";
        }


        public class Description : IVertexDescription
        {
            public int Size => 44;
            public InputElement[] Elements => _elements;

            private static InputElement[] _elements = new[] {
                new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                new InputElement("NORMAL", 0, Format.R32G32B32_Float, 16, 0),
                new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 28, 0),
            };
        }
    }
}
