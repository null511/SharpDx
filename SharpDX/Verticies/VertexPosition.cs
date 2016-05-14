using SharpDX.Core.Geometry;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System.Runtime.InteropServices;

namespace SharpDX.Verticies
{
    [StructLayout(LayoutKind.Explicit, Size = 16)]
    struct VertexPosition
    {
        public static IVertexDescription Info = new Description();

        [FieldOffset(0)]
        public Vector4 Position;


        public VertexPosition(Vector3 position) {
            this.Position = new Vector4(position, 1f);
        }

        public VertexPosition(Vector4 position) {
            this.Position = position;
        }

        public override string ToString() {
            return $"X:{Position.X} Y:{Position.Y} Z:{Position.Z} W:{Position.W}";
        }


        public class Description : IVertexDescription
        {
            public int Size => 16;
            public InputElement[] Elements => _elements;

            private static InputElement[] _elements = new[] {
                new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
            };
        }
    }
}
