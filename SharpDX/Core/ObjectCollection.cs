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
        protected readonly ShaderMeshDictionary<Mesh, List<TObject>> _objects;

        public readonly List<TObject> All;


        public ObjectCollection() {
            All = new List<TObject>();
            _objects = new ShaderMeshDictionary<Mesh, List<TObject>>();
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
                .Each(i => i.Dispose());
        }

        public virtual bool Any() {
            return All.Count > 0;
        }

        public virtual void Add(TObject @object, IShader shader, Mesh mesh) {
            GetObjectList(shader, mesh).Add(@object);
            All.Add(@object);
        }

        public virtual void Remove(TObject item, IShader shader, Mesh mesh) {
            _objects.Get(shader, mesh)?.Remove(item);
        }

        public virtual void Clear() {
            All.Clear();
            _objects.Clear();
        }

        public abstract int Render(DeviceContext context);

        protected List<TObject> GetObjectList(IShader shader, Mesh mesh) {
            return _objects.Get(shader, mesh, () => new List<TObject>());
        }
    }
}
