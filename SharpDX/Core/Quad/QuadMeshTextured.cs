using SharpDX.Core.Geometry;
using SharpDX.Direct3D11;
using SharpDX.Verticies;

namespace SharpDX.Core.Quad
{
    class QuadMeshTextured : Mesh
    {
        public QuadMeshTextured(DeviceContext context, float depth) {
            PrimitiveTopology = Direct3D.PrimitiveTopology.TriangleList;

            ushort[] i;
            var vertices = new[] {
                new VertexPositionTexture(new Vector3(0f, 1f, depth), new Vector2(0f, 1f)),
                new VertexPositionTexture(new Vector3(1f, 1f, depth), new Vector2(1f, 1f)),
                new VertexPositionTexture(new Vector3(0f, 0f, depth), new Vector2(0f, 0f)),
                new VertexPositionTexture(new Vector3(1f, 0f, depth), new Vector2(1f, 0f)),
            };

            var MeshBuilder = new MeshBuilder<VertexPositionTexture, ushort>(VertexPositionTexture.Info);
            MeshBuilder.AppendVerticies(ref vertices, out i);
            MeshBuilder.AppendIndicies(i[2], i[1], i[0], i[1], i[2], i[3]);
            MeshBuilder.Build(context, this);
        }
    }
}
