using SharpDX.Core.Geometry;
using SharpDX.Core.SceneTree;
using SharpDX.Data;
using SharpDX.Direct3D11;
using SharpDX.Filters;
using SharpDX.Geometry;
using SharpDX.Test;
using SharpDX.Verticies;
using System;

namespace SharpDX.Scenes
{
    class TestScene
    {
        private const float cubeSize = 6f;

        private SceneGraph sceneGraph;
        private Camera _camera;
        private Input _input;

        public int EntityCount => sceneGraph.EntityCount;
        public int RenderCount => sceneGraph.RenderCount;


        public TestScene() {
            sceneGraph = new SceneGraph(new TreeDescription {
                MaxLevels = 6,
                RegionCountX = 8,
                RegionCountY = 2,
                RegionCountZ = 8,
                CubeSize = new Vector3(cubeSize),
            });

            _camera = new Camera();
            _input = new Input();
            _input.OnScroll += Input_OnScroll;
        }

        public void Dispose() {
            Utilities.Dispose(ref sceneGraph);
        }

        public void Create(DeviceContext context) {
            var filter = new OutlineFilter();

            sceneGraph.Create(context, _camera, new Vector3(cubeSize), filter);
        }

        private void Input_OnScroll(int dir) {
            if (dir > 0) sceneGraph.Increase();
            else if (dir < 0) sceneGraph.Decrease();
        }

        public void Resize() {
            _camera.InvalidateProjection();
        }

        public void Update(float time) {
            _input.Update();

            _camera.Pan += _input.MouseForce.X * 0.2f;
            _camera.Tilt += _input.MouseForce.Y * 0.2f;

            if (_input.MoveAny) {
                Vector3 moveOut, moveIn = new Vector3(_input.MoveX, 0, _input.MoveZ);
                Vector3.TransformNormal(ref moveIn, ref _camera.WorldMatrix, out moveOut);
                moveOut *= (_input.Shift ? 100f : 20f) * time;

                _camera.Position += moveOut;
            }
            _camera.InvalidateView();

            _camera.Update();
            sceneGraph.Update();
        }

        public void Render(DeviceContext context) {
            sceneGraph.Render(context, _camera);
        }
    }
}
