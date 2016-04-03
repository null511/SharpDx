using SharpDX.Core.Filters;
using SharpDX.Core.Quad;
using SharpDX.Core.SceneTree;
using SharpDX.Core.Text;
using SharpDX.Data;
using SharpDX.Direct3D11;
using SharpDX.Filters;
using SharpDX.Test;
using System;

namespace SharpDX.Scenes
{
    class TestScene : IDisposable
    {
        private const float cubeSize = 10f;

        private SceneGraph sceneGraph;
        private Camera _camera;
        private Input _input;

        private QuadManager _quadMgr;
        private DebugText _debugText;
        private BlendState _textBlendState, _blendState;
        private DepthStencilState _textDepthState, _depthState;


        public TestScene() {
            var description = new TreeDescription {
                MaxLevels = 6,
                RegionCountX = 8,
                RegionCountY = 2,
                RegionCountZ = 8,
                CubeSize = new Vector3(cubeSize),
            };

            sceneGraph = new SceneGraph(description);
            _quadMgr = new QuadManager();
            _debugText = new DebugText();
            _camera = new Camera();
            _input = new Input();
            _input.OnScroll += Input_OnScroll;
        }

        public void Dispose() {
            Utilities.Dispose(ref _quadMgr);
            Utilities.Dispose(ref sceneGraph);
            Utilities.Dispose(ref _debugText);
            Utilities.Dispose(ref _textBlendState);
            Utilities.Dispose(ref _textDepthState);
        }

        public void Initialize(DeviceContext context) {
            _quadMgr.Initialize(context);

            _debugText.Initialize(Program.TextDevice);
            _quadMgr.Add(_debugText.Quad);

            var filters = new IFilter[] {
                new OutlineFilter(),
                new RandomFilter(400, 100000),
                new BottomFilter(),
            };

            Program.Fps.OnUpdate += Fps_OnUpdate;

            sceneGraph.Create(context, _camera, new Vector3(cubeSize), filters);

            var textBlendDesc = BlendStateDescription.Default();
            textBlendDesc.RenderTarget[0] = new RenderTargetBlendDescription(
                true,
                BlendOption.SourceAlpha,
                BlendOption.InverseSourceAlpha,
                BlendOperation.Add,
                BlendOption.One,
                BlendOption.Zero,
                BlendOperation.Add,
                ColorWriteMaskFlags.All);


            var textDepthDesc = DepthStencilStateDescription.Default();
            textDepthDesc.IsDepthEnabled = false;

            _blendState = new BlendState(context.Device, BlendStateDescription.Default());
            _depthState = new DepthStencilState(context.Device, DepthStencilStateDescription.Default());
            _textBlendState = new BlendState(context.Device, textBlendDesc);
            _textDepthState = new DepthStencilState(context.Device, textDepthDesc);
        }

        private void Fps_OnUpdate(int value) {
            _debugText.SetFps(value);
            _debugText.SetRenderCount(sceneGraph.RenderCount);
            _debugText.SetEntityCount(sceneGraph.EntityCount);
        }

        private void Input_OnScroll(int dir) {
            if (dir > 0) sceneGraph.Increase();
            else if (dir < 0) sceneGraph.Decrease();
        }

        public void Resize() {
            _camera.InvalidateProjection();
            _debugText.Resize();
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
            context.OutputMerger.BlendState = _blendState;
            context.OutputMerger.DepthStencilState = _depthState;

            sceneGraph.Render(context, _camera);

            context.OutputMerger.BlendState = _textBlendState;
            context.OutputMerger.DepthStencilState = _textDepthState;

            _quadMgr.Render(context);
        }

        public void RenderText(TextDevice context) {
            _debugText.Render(context);
        }
    }
}
