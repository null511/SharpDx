namespace SharpDX.Core.Entities
{
    class Entity
    {
        public Vector3 Position;
        public BoundingBox Bounds, WorldBounds;
        public Matrix World;

        protected bool _isWorldValid;


        public Entity() {
            World = Matrix.Identity;
        }

        public virtual void Update() {
            if (!_isWorldValid) {
                UpdateWorld();
                _isWorldValid = true;
            }
        }

        protected virtual void UpdateWorld() {
            WorldBounds.Minimum = Bounds.Minimum + Position;
            WorldBounds.Maximum = Bounds.Maximum + Position;

            Matrix.Translation(ref Position, out World);
        }

        public void InvalidateWorld() {
            _isWorldValid = false;
        }
    }
}