using System.Collections.Generic;

namespace SharpDX.Core.SceneTree
{
    class TestOptions
    {
        public BoundingFrustum Frustum;
        public bool EnableDebugCubeRendering;
        public int DebugCubeRenderLevel;

        public IList<RenderCube> debugCubeList;
    }
}
