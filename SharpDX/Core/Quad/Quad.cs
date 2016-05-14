namespace SharpDX.Core.Quad
{
    abstract class Quad : IObject
    {
        public bool Visible;
        public Vector2 Position, Size;


        public Quad() {
            Visible = true;
        }

        public Quad SetPosition(int x, int y) {
            Position.X = x / (float)Program.FormWidth;
            Position.Y = y / (float)Program.FormHeight;
            return this;
        }

        public Quad SetPosition(float x, float y) {
            Position.X = x;
            Position.Y = y;
            return this;
        }

        public Quad SetPosition(ref Vector2 position) {
            Position.X = position.X;
            Position.Y = position.Y;
            return this;
        }

        public Quad SetSize(int width, int height) {
            Size.X = width / (float)Program.FormWidth;
            Size.Y = height / (float)Program.FormHeight;
            return this;
        }

        public Quad SetSize(float x, float y) {
            Size.X = x;
            Size.Y = y;
            return this;
        }

        public Quad SetSize(ref Vector2 size) {
            Size.X = size.X;
            Size.Y = size.Y;
            return this;
        }

        public Quad SetRect(ref Rectangle rect) {
            Position.X = rect.X / (float)Program.FormWidth;
            Position.Y = rect.Y / (float)Program.FormHeight;
            Size.X = rect.Width / (float)Program.FormWidth;
            Size.Y = rect.Height / (float)Program.FormHeight;
            return this;
        }
    }
}
