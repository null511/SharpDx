using SharpDX.Mathematics.Interop;
using System;
using System.Windows.Forms;

namespace SharpDX.UI.Controls
{
    class Button : ContainerBase
    {
        public event MouseOverChangedEvent MouseOverChanged;
        public event EventHandler Click, Focus, Blur;

        private bool _isMouseOver;
        private bool _isPressed;
        private Color4 _color;
        private Color4 _colorOver;

        public Cursor Cursor;

        public Color4 Color {
            get {return _color;}
            set {
                if (_color == value) return;
                _color = value;
                UpdateColor();
            }
        }

        public Color4 ColorOver {
            get {return _colorOver;}
            set {
                if (_colorOver == value) return;
                _colorOver = value;
                UpdateColor();
            }
        }

        public bool IsMouseOver {
            get {return _isMouseOver;}
            set {
                if (_isMouseOver == value) return;
                _isMouseOver = value;
                InvokeMouseOverChanged();
            }
        }

        public bool IsPressed {
            get {return _isPressed;}
            set {
                if (_isPressed == value) return;
                _isPressed = value;
                if (value) OnClick();
                //InvokePressedChanged();
            }
        }


        public Button() : base(false) {
            Color = new RawColor4(1f, 0f, 0f, 1f);
            ColorOver = new RawColor4(0f, 1f, 0f, 1f);
        }

        public override void Update(UiUpdateEventArgs e) {
            IsMouseOver = GetIsMouseOver(e);
            IsPressed = IsMouseOver && e.Input.MouseLeft;
        }

        public bool GetIsMouseOver(UiUpdateEventArgs e) {
            if (e.MouseX < ScreenPosition.X || e.MouseX >= ScreenPosition.X + Size.X) return false;
            if (e.MouseY < ScreenPosition.Y || e.MouseY >= ScreenPosition.Y + Size.Y) return false;
            return true;
        }

        protected void UpdateColor() {
            if (IsMouseOver)
                Children.SetBackgroundColor(ref _colorOver);
            else
                Children.SetBackgroundColor(ref _color);
        }

        protected void InvokeMouseOverChanged() {
            if (IsMouseOver)
                OnFocus();
            else
                OnBlur();

            MouseOverChanged?.Invoke(this);
        }

        protected void OnClick() {
            Click?.Invoke(this, new EventArgs());
        }

        protected void OnFocus() {
            if (Cursor != null)
                Program.Form.Cursor = Cursor;

            UpdateColor();
            Focus?.Invoke(this, new EventArgs());
        }

        protected void OnBlur() {
            if (Cursor != null)
                Program.Form.Cursor = Cursors.Default;

            UpdateColor();
            Blur?.Invoke(this, new EventArgs());
        }
    }

    static class ButtonUtils
    {
        public static T SetColor<T>(this T button, Color4 color)
            where T : Button
        {
            button.Color = color;
            return button;
        }

        public static T SetHoverColor<T>(this T button, Color4 color)
            where T : Button
        {
            button.ColorOver = color;
            return button;
        }

        public static T SetCursor<T>(this T button, Cursor cursor)
            where T : Button
        {
            button.Cursor = cursor;
            return button;
        }

        public static T OnClick<T>(this T button, EventHandler e)
            where T : Button
        {
            button.Click += e;
            return button;
        }
    }
}
