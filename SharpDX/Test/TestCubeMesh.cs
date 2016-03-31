using SharpDX.Core.Geometry;
using SharpDX.Direct3D11;
using SharpDX.Geometry;
using SharpDX.Verticies;

namespace SharpDX.Test
{
    class TestCubeMesh : InstancedMesh
    {
        public TestCubeMesh(DeviceContext context, Vector3 cubeSize) {
            PrimitiveTopology = Direct3D.PrimitiveTopology.LineList;

            var MeshBuilder = new MeshBuilder<VertexTestCube, ushort>(VertexTestCube.Info);
            MeshBuilder.AppendCubeLH(cubeSize, (p, n) => new VertexTestCube(p, n, Vector2.Zero));
            MeshBuilder.Build(context, this);

            OnBuildInstance<TestCube, TestCubeMeshInstance>(OnBuildInstance);
        }

        private void OnBuildInstance(TestCube entity, ref TestCubeMeshInstance instance) {
            Matrix.Transpose(ref entity.World, out instance.matWorld);
            instance.Color = entity.Color;
        }
    }

    struct TestCubeMeshInstance
    {
        public Matrix matWorld;
        public Color4 Color;
    }
}
