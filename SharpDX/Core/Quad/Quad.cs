namespace SharpDX.Core.Quad
{
    abstract class Quad : IObject
    {
        public bool Visible;
        public Vector2 Position, Size;


        public Quad() {
            Visible = true;
        }

        public void SetPosition(int x, int y) {
            Position.X = x / (float)Program.FormWidth;
            Position.Y = y / (float)Program.FormHeight;
        }

        public void SetPosition(float x, float y) {
            Position.X = x;
            Position.Y = y;
        }

        public void SetPosition(ref Vector2 position) {
            Position.X = position.X;
            Position.Y = position.Y;
        }

        public void SetSize(int width, int height) {
            Size.X = width / (float)Program.FormWidth;
            Size.Y = height / (float)Program.FormHeight;
        }

        public void SetSize(float x, float y) {
            Size.X = x;
            Size.Y = y;
        }

        public void SetSize(ref Vector2 size) {
            Size.X = size.X;
            Size.Y = size.Y;
        }

        public void SetRect(ref Rectangle rect) {
            Position.X = rect.X / Program.FormWidth;
            Position.Y = rect.Y / Program.FormHeight;
            Size.X = rect.Width / Program.FormWidth;
            Size.Y = rect.Height / Program.FormHeight;
        }
    }
}
