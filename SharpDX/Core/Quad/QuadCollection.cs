using SharpDX.Core.Geometry;
using SharpDX.Core.Shaders;
using SharpDX.Direct3D11;

namespace SharpDX.Core.Quad
{
    class QuadCollection : ObjectCollection<Quad>
    {
        public Mesh Mesh;


        public override int Render(DeviceContext context) {
            IShader shader;
            int quadCount;
            Quad quad;
            Mesh mesh;

            var renderCount = 0;
            foreach (var shaderKey in _objects) {
                shader = shaderKey.Key;
                shader.Apply(context);

                foreach (var meshKey in shaderKey.Value) {
                    mesh = meshKey.Key;
                    mesh.Apply(context);

                    quadCount = meshKey.Value.Count;
                    for (int i = 0; i < quadCount; i++) {
                        quad = meshKey.Value[i];
                        if (!quad.Visible) continue;

                        shader.ActionRegistry.Apply(quad);
                        shader.Update(context);

                        mesh.Render(context);
                        renderCount++;
                    }
                }
            }

            return renderCount;
        }
    }
}
