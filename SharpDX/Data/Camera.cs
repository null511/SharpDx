using SharpDX.Core;

namespace SharpDX.Data
{
    class Camera : View
    {
        private static Vector3 _up = Vector3.UnitY;

        private Matrix _rot;
        private Vector3 _lookAt;

        public float ClipNear {
            get {return ClipRange.X;}
            set {ClipRange.X = value;}
        }

        public float ClipFar {
            get {return ClipRange.Y;}
            set {ClipRange.Y = value;}
        }


        public Camera() {
            ClipRange = new Vector2(0.1f, 1000f);
        }

        protected override void UpdateViewMatrix() {
            var _yaw = MathUtil.DegreesToRadians(Pan);
            var _pitch = MathUtil.DegreesToRadians(Tilt);
            var _roll = MathUtil.DegreesToRadians(Roll);
            Matrix.RotationYawPitchRoll(_yaw, _pitch, _roll, out _rot);

            Matrix _t;
            Matrix.Translation(ref Position, out _t);
            Matrix.Multiply(ref _rot, ref _t, out WorldMatrix);

            var forward = Vector3.UnitZ;
            Vector3.Transform(ref forward, ref _rot, out _lookAt);
            _lookAt += Position;

            Matrix.LookAtLH(ref Position, ref _lookAt, ref _up, out ViewMatrix);
        }

        protected override void UpdateProjectionMatrix() {
            var fov = MathUtil.PiOverFour;
            var aspect = Program.FormWidth / (float)Program.FormHeight;
            Matrix.PerspectiveFovLH(fov, aspect, ClipRange.X, ClipRange.Y, out ProjectionMatrix);
        }
    }
}
