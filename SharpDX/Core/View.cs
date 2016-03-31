namespace SharpDX.Core
{
    class View
    {
        public BoundingFrustum Frustum;
        public Matrix WorldMatrix, ViewMatrix, ProjectionMatrix;
        public Vector3 Position;
        public Vector2 ClipRange;
        public float Pan, Tilt, Roll;

        private bool _isViewValid, _isProjectionValid, _isFrustumValid;
        private Matrix _vp;


        public View() {
            Frustum = new BoundingFrustum();
        }

        public virtual void Update() {
            if (!_isViewValid) {
                UpdateViewMatrix();
                _isViewValid = true;
                _isFrustumValid = false;
            }

            if (!_isProjectionValid) {
                UpdateProjectionMatrix();
                _isProjectionValid = true;
                _isFrustumValid = false;
            }

            if (!_isFrustumValid) {
                UpdateFrustum();
                _isFrustumValid = true;
            }
        }

        public void InvalidateView() {
            _isViewValid = false;
        }

        public void InvalidateProjection() {
            _isProjectionValid = false;
        }

        protected virtual void UpdateViewMatrix() {
            WorldMatrix = ViewMatrix = Matrix.Identity;
        }

        protected virtual void UpdateProjectionMatrix() {
            ProjectionMatrix = Matrix.Identity;
        }

        protected virtual void UpdateFrustum() {
            Matrix.Multiply(ref ViewMatrix, ref ProjectionMatrix, out _vp);
            Frustum.Matrix = _vp;
        }
    }
}
