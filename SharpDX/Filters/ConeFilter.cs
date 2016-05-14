using SharpDX.Core.Filters;
using SharpDX.Test;

namespace SharpDX.Filters
{
    class ConeFilter : IFilter
    {
        private Vector3 Position;
        private Vector3 Direction;
        private float Radius, Height;


        public ConeFilter(Vector3 position, Vector3 direction, float height, float radius) {
            this.Position = position;
            this.Direction = direction;
            this.Height = height;
            this.Radius = radius;
        }

        public bool Test(TreeBuilderTestEventArgs e) {
            float cone_dist;
            Vector3 offset;

            Vector3.Subtract(ref e.Position, ref Position, out offset);
            Vector3.Dot(ref offset, ref Direction, out cone_dist);

            if (cone_dist < 0 || cone_dist >= Height) return false;

            var cone_radius = (cone_dist / Height) * Radius;

            var orth_distance = (offset - cone_dist * Direction).Length();

            var x = orth_distance < cone_radius;


            if (x) RandomColor.GetRGB(ref e.Color);
            return x;
        }
    }
}
