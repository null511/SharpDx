using SharpDX.Core.Geometry;
using SharpDX.Direct3D11;
using SharpDX.Geometry;
using SharpDX.Verticies;

namespace SharpDX.Test
{
    class DebugCubeMesh : Mesh
    {
        public DebugCubeMesh(DeviceContext context) {
            PrimitiveTopology = Direct3D.PrimitiveTopology.LineList;

            var meshBuilder = new MeshBuilder<VertexPosition, ushort>(VertexPosition.Info);
            meshBuilder.AppendLineCubeLH(1f, p => new VertexPosition(p));
            meshBuilder.Build(context, this);
        }
    }
}
