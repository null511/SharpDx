using System;
using System.Collections.Generic;
using System.Windows.Forms;
using SysPoint = System.Drawing.Point;

namespace SharpDX.Core
{
    class Input : IDisposable
    {
        public event MouseEventHandler MouseClick;
        public event MouseEventHandler MouseWheel;

        private SysPoint formCenter;
        private SysPoint screenCenter;
        private bool discardMouseForce;
        private bool isDisposed;

        public Vector2 MouseForce;

        public Dictionary<Keys, bool> PressedKeys { get; }
        public bool MouseLeft { get; private set; }
        public bool MouseRight { get; private set; }
        public int MouseX { get; private set; }
        public int MouseY { get; private set; }
        public bool MouseLocked { get; private set; }


        public Input() {
            PressedKeys = new Dictionary<Keys, bool>();

            Program.Form.MouseClick += MouseClick;
            Program.Form.MouseWheel += MouseWheel;
            Program.Form.MouseMove += Form_MouseMove;
            Program.Form.MouseDown += Form_MouseDown;
            Program.Form.MouseUp += Form_MouseUp;
            Program.Form.KeyDown += Form_KeyDown;
            Program.Form.KeyUp += Form_KeyUp;
        }

        public void Dispose() {
            if (isDisposed) return;
            isDisposed = true;
            UnlockMouse();

            Program.Form.MouseClick -= MouseClick;
            Program.Form.MouseWheel += MouseWheel;
            Program.Form.MouseMove -= Form_MouseMove;
            Program.Form.MouseDown -= Form_MouseDown;
            Program.Form.MouseUp -= Form_MouseUp;
            Program.Form.KeyDown -= Form_KeyDown;
            Program.Form.KeyUp -= Form_KeyUp;
        }

        public void Update() {
            if (MouseLocked) {
                var p = Program.Form.PointToClient(Cursor.Position);
                MouseForce.X = p.X - formCenter.X;
                MouseForce.Y = p.Y - formCenter.Y;

                if (discardMouseForce) {
                    MouseForce = Vector2.Zero;
                    discardMouseForce = false;
                }

                // Reset Mouse
                formCenter.X = Program.FormWidth / 2;
                formCenter.Y = Program.FormHeight / 2;
                screenCenter = Program.Form.PointToScreen(formCenter);

                Cursor.Position = screenCenter;
            }
        }

        public int KeyValue(Keys key) {
            return IsKeyPressed(key) ? 1 : 0;
        }

        public bool IsKeyPressed(Keys key) {
            bool value;
            if (PressedKeys.TryGetValue(key, out value))
                return value;
            return false;
        }

        public void LockMouse() {
            if (MouseLocked) return;
            MouseLocked = true;
            discardMouseForce = true;
        }

        public void UnlockMouse() {
            if (!MouseLocked) return;
            MouseLocked = false;
        }

        //private void Form_MouseClick(object sender, MouseEventArgs e) {
        //    MouseClick?.Invoke(sender, e);
        //}

        //private void Form_MouseWheel(object sender, MouseEventArgs e) {
            
        //}

        private void Form_MouseUp(object sender, MouseEventArgs e) {
            switch (e.Button) {
                case MouseButtons.Left:
                    MouseLeft = false;
                    break;
                case MouseButtons.Right:
                    MouseRight = false;
                    break;
            }
        }

        private void Form_MouseDown(object sender, MouseEventArgs e) {
            switch (e.Button) {
                case MouseButtons.Left:
                    MouseLeft = true;
                    break;
                case MouseButtons.Right:
                    MouseRight = true;
                    break;
            }
        }

        private void Form_KeyUp(object sender, KeyEventArgs e) {
            PressedKeys[e.KeyCode] = false;
        }

        private void Form_KeyDown(object sender, KeyEventArgs e) {
            PressedKeys[e.KeyCode] = true;
        }

        private void Form_MouseMove(object sender, MouseEventArgs e) {
            MouseX = e.X;
            MouseY = e.Y;
        }
    }
}
