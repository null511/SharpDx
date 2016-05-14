using SharpDX.Core;
using SharpDX.Core.Scenes;
using SharpDX.UI;
using SharpDX.UI.Controls;
using Label = SharpDX.UI.Controls.Label;

namespace SharpDX.Scenes
{
    partial class MainMenuScene : SceneBase
    {
        private Color4 backgroundColor = new Color4(0.1f, 0.1f, 0.1f, 1f);

        private Rectangle _bounds;
        private BlendStateManager _blendStates;
        private DepthStateManager _depthStates;
        private bool _isLoaded;

        private UiManager _uiMgr;
        private PanelColored _panel;
        private Label lblTitle;
        private TextButton btnNew, btnLoad, btnExit;


        public MainMenuScene() {
            _bounds.Width = 200;
            _bounds.Height = 220;
        }

        public override void Load(Context context) {
            _blendStates = new BlendStateManager();
            _depthStates = new DepthStateManager();

            _uiMgr = new UiManager();
            _uiMgr.Initialize(context);

            LoadUi();

            context.Restore();
            _isLoaded = true;
        }

        public override void Unload() {
            if (_isLoaded) {
                UnloadUi();
                _isLoaded = false;
            }

            Utilities.Dispose(ref _uiMgr);
            Utilities.Dispose(ref _blendStates);
            Utilities.Dispose(ref _depthStates);
        }

        public override void Resize() {
            UpdateBounds();
        }

        public override void Update(float time) {
            _uiMgr.Update(new UiUpdateEventArgs(Input));
        }

        public override void Render(Context context) {
            if (_uiMgr.RenderBuffers(context))
                context.Restore();

            context.Clear(ref backgroundColor);
            context.ClearDepth(1f);

            var i = context.Immediate;
            i.OutputMerger.BlendState = _blendStates.Quad(i);
            i.OutputMerger.DepthStencilState = _depthStates.Quad(i);

            _uiMgr.Render(context);
        }

        private void UpdateBounds() {
            _bounds.X = (Program.FormWidth - _bounds.Width) / 2;
            _bounds.Y = (Program.FormHeight - _bounds.Height) / 2;

            _panel
                .SetScreenPosition(_bounds.X, _bounds.Y)
                .SetScreenSize(_bounds.Width, _bounds.Height);
        }

        private void ActionNew() {
            Program.GoTo<TestScene>();
        }

        private void ActionLoad() {
            Program.GoTo<LoadScene>();
        }

        private void ActionExit() {
            Program.Form.Close();
        }
    }
}
