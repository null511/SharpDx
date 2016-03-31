using SharpDX.DirectInput;
using System.Linq;
using System.Windows.Forms;

namespace SharpDX.Test
{
    class Input
    {
        private const int ScrollThreshold = 6;

        public delegate void ScrollHandler(int direction);
        public event ScrollHandler OnScroll;

        private Mouse _mouse;
        private Keyboard _keyboard;
        private MouseState _mouseState, _mouseStatePrev;
        private KeyboardState _keyState, _keyStatePrev;
        private bool _discardMouseForce;
        public bool MoveForward, MoveBack;
        public bool MoveLeft, MoveRight;
        public Vector2 MouseForce;

        public int MoveZ => (MoveForward?1:0) - (MoveBack?1:0);
        public int MoveX => (MoveRight?1:0) - (MoveLeft?1:0);
        public bool MoveAny => MoveForward || MoveBack || MoveLeft || MoveRight;
        public bool Shift => _keyState.PressedKeys.Any(x => x == Key.LeftShift || x == Key.RightShift);


        public Input() {
            var device = new DirectInput.DirectInput();

            _mouse = new Mouse(device);
            _mouse.Acquire();
            _mouseState = _mouse.GetCurrentState();

            _keyboard = new Keyboard(device);
            _keyboard.Acquire();
            _keyState = _keyboard.GetCurrentState();

            _discardMouseForce = true;
        }

        private System.Drawing.Point formCenter;
        private System.Drawing.Point screenCenter;

        public void Update() {
            // Keyboard
            _keyStatePrev = _keyState;
            _keyState = _keyboard.GetCurrentState();

            MoveForward = _keyState.PressedKeys.Any(x => x == Key.Up || x == Key.W);
            MoveBack = _keyState.PressedKeys.Any(x => x == Key.Down || x == Key.S);
            MoveLeft = _keyState.PressedKeys.Any(x => x == Key.Left || x == Key.A);
            MoveRight = _keyState.PressedKeys.Any(x => x == Key.Right || x == Key.D);

            // Mouse
            _mouseStatePrev = _mouseState;
            _mouseState = _mouse.GetCurrentState();

            var p = Program.Form.PointToClient(Cursor.Position);
            MouseForce.X = p.X - formCenter.X;
            MouseForce.Y = p.Y - formCenter.Y;

            if (_discardMouseForce) {
                MouseForce = Vector2.Zero;
                _discardMouseForce = false;
            }

            if (_mouseState.Z >= ScrollThreshold && _mouseStatePrev.Z < ScrollThreshold)
                OnScroll?.Invoke(1);

            if (_mouseState.Z <= -ScrollThreshold && _mouseStatePrev.Z > -ScrollThreshold)
                OnScroll?.Invoke(-1);

            // Reset Mouse
            formCenter = new System.Drawing.Point();
            formCenter.X = Program.FormWidth / 2;
            formCenter.Y = Program.FormHeight / 2;
            screenCenter = Program.Form.PointToScreen(formCenter);

            Cursor.Position = screenCenter;
        }

        public void DiscardMouseForce() {
            _discardMouseForce = true;
        }
    }
}
