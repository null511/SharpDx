using SharpDX.Core.Geometry;
using SharpDX.Core.Shaders;

namespace SharpDX.Core.Entities
{
    class RenderEntity : Entity
    {
        public IShader Shader;
        public Mesh Mesh;
    }
}
