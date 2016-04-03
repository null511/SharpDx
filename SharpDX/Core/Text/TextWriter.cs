using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using System;

namespace SharpDX.Core.Text
{
    class TextWriter : IDisposable
    {
        private bool _isValid;
        private string _text;

        public Vector2 Position, Size;
        public TextFormat TextFormat;
        public TextLayout TextLayout;

        public string Text => _text;


        public TextWriter(TextDevice context, string fontName = "Calibri", float fontSize = 16) {
            TextFormat = new TextFormat(context.FactoryDWrite, fontName, fontSize) {
                TextAlignment = TextAlignment.Leading,
                ParagraphAlignment = ParagraphAlignment.Near,
                WordWrapping = WordWrapping.NoWrap,
            };
        }

        public void Dispose() {
            Utilities.Dispose(ref TextFormat);
            Utilities.Dispose(ref TextLayout);
        }

        public void SetText(string text) {
            this._text = text;
            _isValid = false;
        }

        public void SetPosition(float x, float y) {
            Position.X = x;
            Position.Y = y;
            _isValid = false;
        }

        public void SetPosition(ref Vector2 position) {
            Position.X = position.X;
            Position.Y = position.Y;
            _isValid = false;
        }

        public void SetSize(float width, float height) {
            Size.X = width;
            Size.Y = height;
            _isValid = false;
        }

        public void SetSize(ref Vector2 size) {
            Size.X = size.X;
            Size.Y = size.Y;
            _isValid = false;
        }

        public void Render(TextDevice context, Brush brush) {
            if (string.IsNullOrEmpty(_text)) return;

            if (!_isValid) {
                if (TextLayout != null) TextLayout.Dispose();

                TextLayout = new TextLayout(context.FactoryDWrite, _text, TextFormat, Size.X, Size.Y);
                _isValid = true;
            }

            context.RenderTarget2D.DrawTextLayout(Position, TextLayout, brush, DrawTextOptions.None);
        }
    }
}
