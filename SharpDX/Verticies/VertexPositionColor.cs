using SharpDX.Core.Geometry;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System.Runtime.InteropServices;

namespace SharpDX.Verticies
{
    [StructLayout(LayoutKind.Explicit, Size = 32)]
    struct VertexPositionColor
    {
        public static IVertexDescription Info = new Description();

        [FieldOffset(0)]
        public Vector4 Position;

        [FieldOffset(16)]
        public Color4 Color;


        public VertexPositionColor(Vector3 position, Color4 color) {
            this.Position = new Vector4(position, 1f);
            this.Color = color;
        }

        public VertexPositionColor(Vector4 position, Color4 color) {
            this.Position = position;
            this.Color = color;
        }

        public override string ToString() {
            return $"X:{Position.X} Y:{Position.Y} Z:{Position.Z} W:{Position.W}"
                +$"R:{Color.Red} G:{Color.Green} B:{Color.Blue} A:{Color.Alpha}";
        }


        public class Description : IVertexDescription
        {
            public int Size => 32;
            public InputElement[] Elements => _elements;

            private static InputElement[] _elements = new[] {
                new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 16, 0),
            };
        }
    }
}
