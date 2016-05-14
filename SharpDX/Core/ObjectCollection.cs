using SharpDX.Core.Entities;
using SharpDX.Core.Geometry;
using SharpDX.Core.Shaders;
using SharpDX.Direct3D11;
using SharpDX.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpDX.Core
{
    abstract class ObjectCollection<TObject> : IDisposable
        where TObject : IObject
    {
        protected readonly ShaderMeshDictionary<Mesh, TObject> _objects;

        public readonly List<TObject> All;


        public ObjectCollection() {
            All = new List<TObject>();
            _objects = new ShaderMeshDictionary<Mesh, TObject>();
        }

        ~ObjectCollection() {
            Dispose(false);
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            All.OfType<IDisposable>()
                .Each(x => x.Dispose());

            Clear();
        }

        public virtual bool Any() {
            return All.Count > 0;
        }

        public virtual void Add(TObject @object, IShader shader, Mesh mesh) {
            _objects.GetOrCreate(shader, mesh).Add(@object);
            All.Add(@object);
        }

        public virtual void Remove(TObject item, IShader shader, Mesh mesh) {
            _objects.TryGet(shader, mesh)?.Remove(item);
            All.Remove(item);
        }

        public virtual void Clear() {
            All.Clear();
            _objects.Clear();
        }

        public abstract int Render(DeviceContext context);
    }
}
