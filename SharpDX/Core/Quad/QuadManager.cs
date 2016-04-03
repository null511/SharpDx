using SharpDX.Core.Geometry;
using SharpDX.Direct3D11;
using SharpDX.Test;
using System;

namespace SharpDX.Core.Quad
{
    class QuadManager : IDisposable
    {
        private Mesh _meshColored;
        private ColoredQuadShader _shaderColored;
        private QuadCollection _items;


        public QuadManager() {
            _items = new QuadCollection();
            _shaderColored = new ColoredQuadShader();
        }

        ~QuadManager() {
            Dispose(false);
        }

        public void Initialize(DeviceContext context) {
            _items.Mesh = _meshColored = QuadMesh.Create(context, 0.5f);

            _shaderColored.Load(context);

            _shaderColored.ActionRegistry.Add<ColoredScreenQuad>(e => {
                _shaderColored.SetPosition(ref e.Position);
                _shaderColored.SetSize(ref e.Size);
                _shaderColored.SetColor(ref e.Color);
            });
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing) {
            _items.Clear();

            _meshColored?.Dispose();
            _shaderColored?.Dispose();
        }

        public void Add(ColoredScreenQuad item) {
            _items.Add(item, _shaderColored, _meshColored);
        }

        public void Remove(ColoredScreenQuad item) {
            _items.Remove(item, _shaderColored, _meshColored);
        }

        public void Render(DeviceContext context) {
            _items.Render(context);
        }
    }
}
