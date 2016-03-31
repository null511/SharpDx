using SharpDX.Core.Entities;

namespace SharpDX.Core.SceneTree
{
    class RenderCube : RenderEntity
    {
        private Matrix _s, _t;

        public Color4 Color;


        protected override void UpdateWorld() {
            WorldBounds.Minimum = Bounds.Minimum + Position;
            WorldBounds.Maximum = Bounds.Maximum + Position;

            Vector3 size;
            Vector3.Subtract(ref WorldBounds.Maximum, ref WorldBounds.Minimum, out size);
            Matrix.Scaling(ref size, out _s);
            Matrix.Translation(ref WorldBounds.Minimum, out _t);
            Matrix.Multiply(ref _s, ref _t, out World);
        }
    }
}
