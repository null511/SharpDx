using SharpDX.Core.Geometry;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System.Runtime.InteropServices;

namespace SharpDX.Verticies
{
    [StructLayout(LayoutKind.Explicit, Size = 32)]
    struct VertexPositionTexture
    {
        public static IVertexDescription Info = new Description();

        [FieldOffset(0)]
        public Vector4 Position;

        [FieldOffset(16)]
        public Vector2 Texcoord;


        public VertexPositionTexture(Vector3 position, Vector2 texcoord) {
            this.Position = new Vector4(position, 1f);
            this.Texcoord = texcoord;
        }

        public VertexPositionTexture(Vector4 position, Vector2 texcoord) {
            this.Position = position;
            this.Texcoord = texcoord;
        }

        public override string ToString() {
            return $"X:{Position.X} Y:{Position.Y} Z:{Position.Z} W:{Position.W}"
                +$"tX:{Texcoord.X} tY:{Texcoord.Y}";
        }


        public class Description : IVertexDescription
        {
            public int Size => 32;
            public InputElement[] Elements => _elements;

            private static InputElement[] _elements = new[] {
                new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                new InputElement("TEXCOORD", 0, Format.R32G32_Float, 16, 0),
            };
        }
    }
}
