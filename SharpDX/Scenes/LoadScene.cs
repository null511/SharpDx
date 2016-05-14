using SharpDX.Core;
using SharpDX.Core.Quad;
using SharpDX.Core.Scenes;
using SharpDX.DirectWrite;
using SharpDX.UI;
using System.Windows.Forms;
using Label = SharpDX.UI.Controls.Label;

namespace SharpDX.Scenes
{
    partial class LoadScene : SceneBase
    {
        private Rectangle _bounds;
        private QuadColored _quad;
        private BlendStateManager _blendStates;
        private DepthStateManager _depthStates;
        private UiManager _uiMgr;
        private Label lblTitle;
        private bool _isLoaded;


        public LoadScene() {
            _bounds.Width = 600;
            _bounds.Height = 400;
        }

        public override void Load(Context context) {
            _blendStates = new BlendStateManager();
            _depthStates = new DepthStateManager();

            _uiMgr = new UiManager();
            _uiMgr.Initialize(context);

            _quad = new QuadColored();
            _quad.Color = new Color4(0.2f, 0.2f, 0.2f, 1f);
            _quad.SetSize(_bounds.Width, _bounds.Height);

            lblTitle = new Label("Impact", 42f)
                .SetHorizontalAlignment(TextAlignment.Center)
                .SetVerticalAlignment(ParagraphAlignment.Center)
                .SetColor(1f, 0f, 0f, 1f)
                .SetSize(_bounds.Width, 40)
                .SetText("SharpDX");

            //_uiMgr.Add();
            _uiMgr.Add(lblTitle);

            Program.Form.KeyDown += Form_KeyDown;

            var i = context.Immediate;
            i.OutputMerger.BlendState = _blendStates.Text(i);
            i.OutputMerger.DepthStencilState = _depthStates.Text(i);
            _isLoaded = true;
        }

        private void Form_KeyDown(object sender, KeyEventArgs e) {
            switch (e.KeyCode) {
                case Keys.Escape:
                    Program.GoTo<MainMenuScene>();
                    break;
            }
        }

        public override void Unload() {
            if (_isLoaded) {
                Program.Form.KeyDown -= Form_KeyDown;
                _isLoaded = false;
            }

            Utilities.Dispose(ref _blendStates);
            Utilities.Dispose(ref _depthStates);
            Utilities.Dispose(ref _uiMgr);
            Utilities.Dispose(ref lblTitle);
        }

        public override void Resize() {
            UpdateBounds();
        }

        public override void Render(Context context) {
            //_uiMgr.Render(context, _quad);

            _uiMgr.Render(context);
        }

        private void UpdateBounds() {
            _bounds.X = (Program.FormWidth - _bounds.Width) / 2;
            _bounds.Y = (Program.FormHeight - _bounds.Height) / 2;

            _quad.SetPosition(_bounds.X, _bounds.Y);

            lblTitle.SetPosition(_bounds.X, _bounds.Y+6);
        }
    }
}
