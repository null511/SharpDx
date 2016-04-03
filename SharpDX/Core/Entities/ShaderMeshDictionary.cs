using SharpDX.Core.Geometry;
using SharpDX.Core.Shaders;
using System;
using System.Collections.Generic;

namespace SharpDX.Core.Entities
{
    class ShaderMeshDictionary<TMesh, T> : ShaderDictionary<IDictionary<TMesh, T>>
        where TMesh : Mesh
    {
        public T Get(IShader shader, TMesh mesh) {
            var shaderKey = base.Get(shader);
            if (shaderKey == null) return default(T);

            T value;
            if (shaderKey.TryGetValue(mesh, out value)) return value;
            return default(T);
        }

        public T Get(IShader shader, TMesh mesh, Func<T> newEvent) {
            var shaderKey = base.Get(shader, () => new Dictionary<TMesh, T>());

            T value;
            if (shaderKey.TryGetValue(mesh, out value)) return value;

            value = newEvent();
            shaderKey.Add(mesh, value);
            return value;
        }
    }
}
