using SharpDX.Core.Geometry;
using System;

namespace SharpDX.Geometry
{
    static class LineCube
    {
        public static MeshBuilder<TVertex, ushort> AppendLineCubeLH<TVertex>(this MeshBuilder<TVertex, ushort> builder, float size, Func<Vector3, TVertex> createPoint)
            where TVertex : struct
        {
            return builder.AppendLineCubeLH(new Vector3(size, size, size), createPoint);
        }

        public static MeshBuilder<TVertex, ushort> AppendLineCubeLH<TVertex>(this MeshBuilder<TVertex, ushort> builder, Vector3 size, Func<Vector3, TVertex> createPoint)
            where TVertex : struct
        {
            var x = size.X;
            var y = size.Y;
            var z = size.Z;

            var vertices = new TVertex[] {
                createPoint(new Vector3(0, y, z)),
                createPoint(new Vector3(x, y, z)),
                createPoint(new Vector3(0, y, 0)),
                createPoint(new Vector3(x, y, 0)),
                createPoint(new Vector3(0, 0, z)),
                createPoint(new Vector3(x, 0, z)),
                createPoint(new Vector3(0, 0, 0)),
                createPoint(new Vector3(x, 0, 0)),
            };

            ushort[] i;
            builder.AppendVerticies(ref vertices, out i);
            builder.AppendIndicies(
                // Top
                i[0], i[1],
                i[1], i[3],
                i[3], i[2],
                i[2], i[0],

                // Bottom
                i[4], i[5],
                i[5], i[7],
                i[7], i[6],
                i[6], i[4],

                // Vertical Corners
                i[0], i[4],
                i[1], i[5],
                i[3], i[7],
                i[2], i[6]);

            return builder;
        }
    }
}
