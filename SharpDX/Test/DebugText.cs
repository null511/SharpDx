using SharpDX.Core.Quad;
using SharpDX.Core.Text;
using System;

namespace SharpDX.Test
{
    class DebugText : IDisposable
    {
        public ColoredScreenQuad Quad;

        private TextWriter _textWriter;
        private Direct2D1.Brush _textBrush;
        private int _fps;
        private int _renderCount;
        private int _entityCount;
        private bool _isValid;


        public DebugText() {
            Quad = new ColoredScreenQuad();
        }

        ~DebugText() {
            Dispose(false);
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing) {
            Utilities.Dispose(ref _textWriter);
        }

        public void Initialize(TextDevice device) {
            Quad.Color = new Color4(0f, 0f, 0f, 0.4f);

            _textWriter = new TextWriter(Program.TextDevice);
        }

        public void Resize() {
            var rect = new Rectangle(8, 8, 200, 64);
            Quad.SetPosition(8, 8);
            Quad.SetSize(200, 64);

            _textWriter.SetPosition(12f, 8f);
            _textWriter.SetSize(192f, 64f);
        }

        public void Render(TextDevice context) {
            if (_textBrush == null)
                _textBrush = context.GetBrush(1f, 1f, 1f, 1f);

            if (!_isValid) {
                var text = $"FPS: {_fps}\n"
                    +$"Entity Count: {_entityCount}\n"
                    +$"Render Count: {_renderCount}";

                _textWriter.SetText(text);
                _isValid = true;
            }

            _textWriter.Render(context, _textBrush);
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
    }
}
