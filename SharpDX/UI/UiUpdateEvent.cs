using SharpDX.Core;

namespace SharpDX.UI
{
    class UiUpdateEventArgs
    {
        public Input Input { get; }
        public float MouseX { get; }
        public float MouseY { get; }

        public UiUpdateEventArgs(Input input) {
            Input = input;

            MouseX = input.MouseX / (float)Program.FormWidth;
            MouseY = input.MouseY / (float)Program.FormHeight;
        }
    }
}
