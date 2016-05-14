using SharpDX.Core;
using SharpDX.Core.Filters;
using SharpDX.Core.Scenes;
using SharpDX.Core.SceneTree;
using SharpDX.Data;
using SharpDX.Filters;
using SharpDX.Test;
using SharpDX.UI;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SharpDX.Scenes
{
    class TestScene : SceneBase
    {
        private const float cubeSize = 1f;

        private Color4 backgroundColor = new Color4(0.0f, 0.1f, 0.3f, 1f);

        private SceneGraph sceneGraph;
        private Camera _camera;
        private UiManager _uiMgr;
        private DebugText _debugText;
        private BlendStateManager _blendStates;
        private DepthStateManager _depthStates;
        private TreeDescription _description;
        private bool _isLoaded;


        public TestScene() {
            _description = new TreeDescription {
                MaxLevels = 7,
                BatchLevel = 3,
                RegionCountX = 8,
                RegionCountY = 2,
                RegionCountZ = 8,
                CubeSize = new Vector3(cubeSize),
            };

            _camera = new Camera();
            _camera.ClipFar = 100f;

            _uiMgr = new UiManager();
            _blendStates = new BlendStateManager();
            _depthStates = new DepthStateManager();
        }

        public override void Load(Context context) {
            sceneGraph = new SceneGraph(_description);

            _uiMgr.Initialize(context);

            _debugText = new DebugText();
            _uiMgr.Add(_debugText);

            var filters = new List<IFilter> {
                new OutlineAllFilter(),
                new BottomFilter(),
                new ConeFilter(Vector3.Zero, Vector3.Down, 1000f, 1000f),
                //new RandomFilter(400, 100000),
            };

            Vector3 size;
            _description.CalculateSize(out size);
            var min = -size * 0.5f;
            var max = size * 0.5f;

            Vector3 pos;
            var r = new Random();
            for (int i = 0; i < 8; i++) {
                pos.X = r.NextFloat(min.X, max.X);
                pos.Y = r.NextFloat(min.Y, max.Y);
                pos.Z = r.NextFloat(min.Z, max.Z);
                filters.Add(new ConeFilter(pos, Vector3.Down, 1000f, 1000f));
            }

            Program.Fps.OnUpdate += Fps_OnUpdate;

            sceneGraph.Create(context.Immediate, _camera, new Vector3(cubeSize), filters.ToArray());

            Input.MouseWheel += Input_MouseWheel;
            Input.LockMouse();

            _isLoaded = true;
        }

        public override void Unload() {
            if (_isLoaded) {
                Input.UnlockMouse();
                Program.Fps.OnUpdate -= Fps_OnUpdate;
                Program.Form.KeyDown -= Form_KeyDown;
                _isLoaded = false;
            }

            Utilities.Dispose(ref _uiMgr);
            Utilities.Dispose(ref _blendStates);
            Utilities.Dispose(ref _depthStates);
            Utilities.Dispose(ref sceneGraph);
        }

        public override void Resize() {
            _camera.InvalidateProjection();

            // UpdateBounds()
        }

        public override void Update(float time) {
            Input.Update();

            _camera.Pan += Input.MouseForce.X * 0.2f;
            _camera.Tilt += Input.MouseForce.Y * 0.2f;

            var MoveForward = Input.IsKeyPressed(Keys.Up) || Input.IsKeyPressed(Keys.W) ? 1 : 0;
            var MoveBack = Input.IsKeyPressed(Keys.Down) || Input.IsKeyPressed(Keys.S) ? 1 : 0;
            var MoveLeft = Input.IsKeyPressed(Keys.Left) || Input.IsKeyPressed(Keys.A) ? 1 : 0;
            var MoveRight = Input.IsKeyPressed(Keys.Right) || Input.IsKeyPressed(Keys.D) ? 1 : 0;

            var moveZ = MoveForward - MoveBack;
            var moveX = MoveLeft - MoveRight;

            if (moveX != 0 || moveZ != 0) {
                Vector3 moveOut, moveIn = new Vector3(moveX, 0, moveZ);
                Vector3.TransformNormal(ref moveIn, ref _camera.WorldMatrix, out moveOut);
                moveOut *= (Input.IsKeyPressed(Keys.Shift) ? 100f : 20f) * time;

                _camera.Position += moveOut;
            }
            _camera.InvalidateView();

            _camera.Update();
            sceneGraph.Update();
        }

        public override void Render(Context context) {
            context.Clear(ref backgroundColor);
            context.ClearDepth(1f);

            var i = context.Immediate;
            i.OutputMerger.BlendState = _blendStates.Default(i);
            i.OutputMerger.DepthStencilState = _depthStates.Default(i);

            sceneGraph.Render(context, _camera);

            i.OutputMerger.BlendState = _blendStates.Quad(i);
            i.OutputMerger.DepthStencilState = _depthStates.Quad(i);

            _uiMgr.Render(context);
        }

        private void Form_KeyDown(object sender, KeyEventArgs e) {
            switch (e.KeyCode) {
                case Keys.Escape:
                    Program.GoTo<MainMenuScene>();
                    break;
            }
        }

        private void Fps_OnUpdate(int value) {
            _debugText.SetFps(value);
            _debugText.SetRenderCount(sceneGraph.RenderCount);
            _debugText.SetEntityCount(sceneGraph.EntityCount);
            _debugText.SetInstanceCount(sceneGraph.InstanceCount);
        }

        private void Input_MouseWheel(object sender, MouseEventArgs e) {
            //if (e.Delta >= ScrollThreshold && _mouseStatePrev.Z < ScrollThreshold)
            //    sceneGraph.Increase();

            //if (e.Delta <= -ScrollThreshold && _mouseStatePrev.Z > -ScrollThreshold)
            //    sceneGraph.Decrease();
        }
    }
}
