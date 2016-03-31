using System;

namespace SharpDX.Core.SceneTree
{
    class NodeArray<T>
    {
        private readonly T[] _items;
        private readonly int _sizeX, _sizeY, _sizeZ;

        public int SizeX => _sizeX;
        public int SizeY => _sizeY;
        public int SizeZ => _sizeZ;


        public NodeArray(int sizeX, int sizeY, int sizeZ) {
            this._sizeX = sizeX;
            this._sizeY = sizeY;
            this._sizeZ = sizeZ;

            var size = sizeX * sizeY * sizeZ;
            _items = new T[size];
        }

        public int GetIndex(int x, int y, int z) {
            Assert_Size(x, 0, _sizeX);
            Assert_Size(y, 0, _sizeY);
            Assert_Size(z, 0, _sizeZ);
            return x + y * _sizeX + z * _sizeY * _sizeX;
        }

        public T Get(int x, int y, int z) {
            var i = GetIndex(x, y, z);
            return _items[i];
        }

        public void Set(int x, int y, int z, T value) {
            var i = GetIndex(x, y, z);
            _items[i] = value;
        }

        private void Assert_Size(int value, int min, int max) {
            if (value < min || value >= max)
                throw new ApplicationException($"Index '{value}' out-of-bounds '{min}' - '{max}'!");
        }
    }
}
