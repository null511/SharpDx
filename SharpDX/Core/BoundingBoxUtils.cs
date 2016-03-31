namespace SharpDX.Core
{
    static class BoundingBoxUtils
    {
        public static void Expand(ref BoundingBox source, ref BoundingBox box) {
            if (box.Minimum.X < source.Minimum.X) source.Minimum.X = box.Minimum.X;
            if (box.Minimum.Y < source.Minimum.Y) source.Minimum.Y = box.Minimum.Y;
            if (box.Minimum.Z < source.Minimum.Z) source.Minimum.Z = box.Minimum.Z;
            if (box.Maximum.X > source.Maximum.X) source.Maximum.X = box.Maximum.X;
            if (box.Maximum.Y > source.Maximum.Y) source.Maximum.Y = box.Maximum.Y;
            if (box.Maximum.Z > source.Maximum.Z) source.Maximum.Z = box.Maximum.Z;
        }

        public static void Expand(ref BoundingBox source, BoundingBox box) {
            Expand(ref source, ref box);
        }
    }
}
