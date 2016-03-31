using SharpDX.Core.Shaders;
using System;
using System.Collections.Generic;

namespace SharpDX.Core.Entities
{
    class ShaderDictionary<T> : Dictionary<IShader, T> {
        public T Get(IShader shader, Func<T> newEvent) {
            T value;
            if (TryGetValue(shader, out value))
                return value;

            value = newEvent();
            Add(shader, value);
            return value;
        }
    }
}
