using SharpDX.Direct3D11;
using System.Collections.Generic;

namespace SharpDX.Core.Geometry
{
    class MeshBuilder<TVertex, TIndex>
        where TVertex : struct
        where TIndex : struct
    {
        private readonly List<TVertex> _vertices;
        private readonly List<TIndex> _indicies;
        private readonly IVertexDescription _vertexDesc;


        public MeshBuilder(IVertexDescription vertexInfo) {
            _vertexDesc = vertexInfo;
            _vertices = new List<TVertex>();
            _indicies = new List<TIndex>();
        }

        public MeshBuilder<TVertex, TIndex> AppendVerticies(ref TVertex[] verticies, out ushort[] indicies) {
            var size = verticies.Length;
            indicies = new ushort[size];

            var x = _vertices.Count;
            for (int i = 0; i < size; i++) {
                indicies[i] = (ushort)(x + i);
            }

            _vertices.AddRange(verticies);
            return this;
        }

        public MeshBuilder<TVertex, TIndex> AppendVerticies(ref TVertex[] verticies, out uint[] indicies) {
            var size = verticies.Length;
            indicies = new uint[size];

            var x = _vertices.Count;
            for (int i = 0; i < size; i++) {
                indicies[i] = (uint)(x + i);
            }

            _vertices.AddRange(verticies);
            return this;
        }

        public MeshBuilder<TVertex, TIndex> AppendIndicies(params TIndex[] index) {
            _indicies.AddRange(index);
            return this;
        }

        public void Build(DeviceContext context, Mesh mesh) {
            mesh.Build(context.Device, _vertices.ToArray(), _indicies.ToArray(), _vertexDesc);
        }
    }
}
