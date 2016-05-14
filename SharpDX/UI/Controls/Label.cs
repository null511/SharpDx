using SharpDX.Core;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using SharpDX.Mathematics.Interop;

namespace SharpDX.UI.Controls
{
    class Label : UiControlBase
    {
        private TextFormat _format;
        private TextLayout _layout;
        private Brush _brush;
        private Color4 _color;
        private TextAlignment _horizontalAlignment;
        private ParagraphAlignment _verticalAlignment;
        private WordWrapping _wrap;
        private string _fontName;
        private float _fontSize;
        private bool _isTextLayoutValid;
        private bool _isTextFormatValid;
        private bool _isTextBrushValid;

        public string Text {get; private set;}


        public Label(string fontName = "Calibri", float fontSize = 16) {
            _fontName = fontName;
            _fontSize = fontSize;

            _horizontalAlignment = TextAlignment.Leading;
            _verticalAlignment = ParagraphAlignment.Near;
            _wrap = WordWrapping.Wrap;
        }

        protected override void Dispose(bool disposing) {
            Utilities.Dispose(ref _brush);
            Utilities.Dispose(ref _format);
            Utilities.Dispose(ref _layout);

            base.Dispose(disposing);
        }

        public Label SetText(string text) {
            this.Text = text;
            _isTextLayoutValid = false;
            return this;
        }

        public Label SetFontName(string name) {
            this._fontName = name;
            _isTextFormatValid = false;
            return this;
        }

        public Label SetFontSize(float size) {
            this._fontSize = size;
            _isTextFormatValid = false;
            return this;
        }

        public Label SetWrap(WordWrapping wrap) {
            this._wrap = wrap;
            _isTextFormatValid = false;
            return this;
        }

        public Label SetColor(ref Color4 color) {
            this._color = color;
            _isTextBrushValid = false;
            return this;
        }

        public Label SetColor(float r, float g, float b, float a) {
            this._color.Red = r;
            this._color.Green = g;
            this._color.Blue = b;
            this._color.Alpha = a;
            _isTextBrushValid = false;
            return this;
        }

        public Label SetHorizontalAlignment(TextAlignment alignment) {
            this._horizontalAlignment = alignment;
            _isTextFormatValid = false;
            return this;
        }

        public Label SetVerticalAlignment(ParagraphAlignment alignment) {
            this._verticalAlignment = alignment;
            _isTextFormatValid = false;
            return this;
        }

        public override void Render(Context context, UiManager uiMgr) {
            if (string.IsNullOrEmpty(Text)) return;

            if (!_isTextFormatValid) {
                Utilities.Dispose(ref _format);

                _format = new TextFormat(context.FactoryDWrite, _fontName, _fontSize) {
                    TextAlignment = _horizontalAlignment,
                    ParagraphAlignment = _verticalAlignment,
                    WordWrapping = _wrap,
                };
                _isTextFormatValid = true;
            }

            if (!_isTextLayoutValid) {
                Utilities.Dispose(ref _layout);

                var w = Program.FormWidth * Size.X;
                var h = Program.FormHeight * Size.Y;
                _layout = new TextLayout(context.FactoryDWrite, Text, _format, w, h);
                _isTextLayoutValid = true;
            }

            if (!_isTextBrushValid) {
                Utilities.Dispose(ref _brush);

                _brush = context.GetBrush(ref _color);
                _isTextBrushValid = true;
            }

            var p = new RawVector2(ScreenPosition.X * Program.FormWidth, ScreenPosition.Y * Program.FormHeight);

            context.BeginText();
            context.RenderTarget2D.DrawTextLayout(p, _layout, _brush, DrawTextOptions.None);
            context.EndText();
        }
    }
}
