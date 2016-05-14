using SharpDX.Core;
using SharpDX.DirectWrite;
using SharpDX.UI;
using SharpDX.UI.Controls;
using Label = SharpDX.UI.Controls.Label;

namespace SharpDX.Test
{
    class DebugText : ContainerBase
    {
        private PanelColored _panel;
        private Label _label;

        private int _fps;
        private int _renderCount;
        private int _entityCount;
        private int _instanceCount;
        private bool _isValid;


        public DebugText() {
            _panel = new PanelColored()
                .SetBackgroundColor(new Color4(0f, 0f, 0f, 0.5f));

            _label = new Label()
                .SetColor(1f, 1f, 1f, 1f)
                .SetSize(152, 80)
                .SetWrap(WordWrapping.NoWrap);

            Children.Add(_panel);
            Children.Add(_label);
        }

        public void Render(Context context) {
            if (!_isValid) {
                var text = $"FPS: {_fps}\n"
                    +$"Entity Count: {_entityCount}\n"
                    +$"Render Count: {_renderCount}\n"
                    +$"Instance Count: {_instanceCount}";

                _label.SetText(text);
                _isValid = true;
            }

            //_textWriter.Render(context);
        }

        public void SetFps(int fps) {
            this._fps = fps;
            _isValid = false;
        }

        public void SetRenderCount(int count) {
            this._renderCount = count;
            _isValid = false;
        }

        public void SetEntityCount(int count) {
            this._entityCount = count;
            _isValid = false;
        }

        public void SetInstanceCount(int count) {
            this._instanceCount = count;
            _isValid = false;
        }
    }
}
