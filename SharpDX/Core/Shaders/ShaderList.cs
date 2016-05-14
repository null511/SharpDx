using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpDX.Core.Shaders
{
    class ShaderList<T> : List<ShaderListItem<T>>, IDisposable
    {
        public void Dispose() {
            foreach (IDisposable i in this.Select(x => x.Data))
                i.Dispose();

            Clear();
        }

        public T Get(IShader shader) {
            var item = GetItem(shader);
            if (item != null) return item.Data;
            return default(T);
        }

        private ShaderListItem<T> GetItem(IShader shader) {
            ShaderListItem<T> item;
            var count = Count;
            for (var i = 0; i < count; i++) {
                item = this[i];
                if (item.Shader == shader)
                    return item;
            }

            return null;
        }

        public T GetOrCreate(IShader shader, Func<T> createFunc) {
            var x = GetItem(shader);
            if (x == null)
                Add(x = new ShaderListItem<T>(shader, createFunc()));

            return x.Data;
        }
    }

    class ShaderListItem<T>
    {
        public readonly IShader Shader;
        public readonly T Data;


        public ShaderListItem(IShader shader, T data) {
            this.Shader = shader;
            this.Data = data;
        }
    }
}
