using SharpDX.Core.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpDX.Core.Entities
{
    class MeshList<TMesh, T> : List<MeshListItem<TMesh, T>>, IDisposable
        where TMesh : Mesh
    {
        public void Dispose() {
            var disposable = this
                .SelectMany(x => x.Items)
                .OfType<IDisposable>();

            foreach (var i in disposable)
                i.Dispose();
        }

        public List<T> Get(TMesh mesh) {
            return GetItem(mesh)?.Items;
        }

        private MeshListItem<TMesh, T> GetItem(TMesh mesh) {
            MeshListItem<TMesh, T> item;
            var count = Count;
            for (var i = 0; i < count; i++) {
                item = this[i];
                if (item.Mesh == mesh)
                    return item;
            }

            return null;
        }

        public List<T> GetOrCreate(TMesh mesh) {
            var x = GetItem(mesh);
            if (x == null)
                Add(x = new MeshListItem<TMesh, T>(mesh));

            return x.Items;
        }

        public void Merge(MeshList<TMesh, T> collection) {
            foreach (var meshKey in collection) {
                var entityList = collection.Get(meshKey.Mesh);
                var thisEntityList = GetOrCreate(meshKey.Mesh);

                thisEntityList.AddRange(entityList);
            }
        }
    }

    class MeshListItem<TMesh, T>
    {
        public readonly TMesh Mesh;
        public List<T> Items;


        public MeshListItem(TMesh mesh) {
            this.Mesh = mesh;

            Items = new List<T>();
        }
    }
}
