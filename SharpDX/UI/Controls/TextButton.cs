using SharpDX.DirectWrite;

namespace SharpDX.UI.Controls
{
    class TextButton : Button
    {
        private Label _label;

        public string Text {
            get {return _label.Text;}
            set {
                if (_label.Text == value) return;
                _label.SetText(value);
                InvokeInvalidated();
            }
        }


        public TextButton() {
            _label = new Label()
                .SetHorizontalAlignment(TextAlignment.Center)
                .SetVerticalAlignment(ParagraphAlignment.Center);

            Children.Add(_label);
        }

        public override void UpdateLayout(Vector2 parentPosition) {
            base.UpdateLayout(parentPosition);

            _label.Size = this.Size;
        }

        public TextButton SetText(string text) {
            Text = text;
            return this;
        }

        public TextButton SetTextColor(Color4 color) {
            _label.SetColor(ref color);
            return this;
        }
    }
}
