using SharpDX.Core.Geometry;
using SharpDX.Direct3D11;
using SharpDX.Verticies;

namespace SharpDX.Core.Quad
{
    class QuadMeshColored : Mesh
    {
        public QuadMeshColored(DeviceContext context, float depth) {
            PrimitiveTopology = Direct3D.PrimitiveTopology.TriangleList;

            ushort[] i;
            var vertices = new[] {
                new VertexPosition(new Vector3(0f, 1f, depth)),
                new VertexPosition(new Vector3(1f, 1f, depth)),
                new VertexPosition(new Vector3(0f, 0f, depth)),
                new VertexPosition(new Vector3(1f, 0f, depth)),
            };

            var MeshBuilder = new MeshBuilder<VertexPosition, ushort>(VertexPosition.Info);
            MeshBuilder.AppendVerticies(ref vertices, out i);
            MeshBuilder.AppendIndicies(i[2], i[1], i[0], i[1], i[2], i[3]);
            MeshBuilder.Build(context, this);
        }
    }
}
