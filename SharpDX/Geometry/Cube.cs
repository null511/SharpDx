using SharpDX.Core.Geometry;
using System;

namespace SharpDX.Geometry
{
    static class Cube
    {
        public static MeshBuilder<TVertex, ushort> AppendCubeLH<TVertex>(this MeshBuilder<TVertex, ushort> builder, float size, Func<Vector3, Vector3, TVertex> createPoint)
            where TVertex : struct
        {
            return builder.AppendCubeLH(new Vector3(size, size, size), createPoint);
        }

        public static MeshBuilder<TVertex, ushort> AppendCubeLH<TVertex>(this MeshBuilder<TVertex, ushort> builder, Vector3 size, Func<Vector3, Vector3, TVertex> createPoint)
            where TVertex : struct
        {
            var x = size.X;
            var y = size.Y;
            var z = size.Z;

            // Back
            builder.CreateCubeFaceLH(new[] {
                new Vector3(0, y, 0),
                new Vector3(x, y, 0),
                new Vector3(0, 0, 0),
                new Vector3(x, 0, 0),
            }, -Vector3.UnitZ, createPoint);

            // Front
            builder.CreateCubeFaceLH(new[] {
                new Vector3(x, y, z),
                new Vector3(0, y, z),
                new Vector3(x, 0, z),
                new Vector3(0, 0, z),
            }, Vector3.UnitZ, createPoint);

            // Left
            builder.CreateCubeFaceLH(new[] {
                new Vector3(0, y, z),
                new Vector3(0, y, 0),
                new Vector3(0, 0, z),
                new Vector3(0, 0, 0),
            }, -Vector3.UnitX, createPoint);

            // Right
            builder.CreateCubeFaceLH(new[] {
                new Vector3(x, y, 0),
                new Vector3(x, y, z),
                new Vector3(x, 0, 0),
                new Vector3(x, 0, z),
            }, Vector3.UnitX, createPoint);

            // Top
            builder.CreateCubeFaceLH(new[] {
                new Vector3(0, y, z),
                new Vector3(x, y, z),
                new Vector3(0, y, 0),
                new Vector3(x, y, 0),
            }, Vector3.UnitY, createPoint);

            // Bottom
            builder.CreateCubeFaceLH(new[] {
                new Vector3(0, 0, 0),
                new Vector3(x, 0, 0),
                new Vector3(0, 0, z),
                new Vector3(x, 0, z),
            }, -Vector3.UnitY, createPoint);

            return builder;
        }

        private static MeshBuilder<TVertex, ushort> CreateCubeFaceLH<TVertex>(this MeshBuilder<TVertex, ushort> builder, Vector3[] points, Vector3 normal, Func<Vector3, Vector3, TVertex> createPoint)
            where TVertex : struct
        {
            var face = new[] {
                createPoint(points[0], normal),
                createPoint(points[1], normal),
                createPoint(points[2], normal),
                createPoint(points[3], normal),
            };

            ushort[] i;
            builder.AppendVerticies(ref face, out i);
            builder.AppendIndicies(i[0], i[1], i[2], i[3], i[2], i[1]);
            return builder;
        }
    }
}
