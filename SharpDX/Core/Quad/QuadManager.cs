using SharpDX.Direct3D11;
using SharpDX.Test;
using System;

namespace SharpDX.Core.Quad
{
    class QuadManager : IDisposable
    {
        private QuadMeshColored _meshColored;
        private QuadMeshTextured _meshTextured;
        private QuadShaderColored _shaderColored;
        private QuadShaderTextured _shaderTextured;
        private QuadCollection _items;


        public QuadManager() {
            _items = new QuadCollection();
            _shaderColored = new QuadShaderColored();
            _shaderTextured = new QuadShaderTextured();
        }

        ~QuadManager() {
            Dispose(false);
        }

        public void Initialize(DeviceContext context) {
            _meshColored = new QuadMeshColored(context, 0.5f);
            _meshTextured = new QuadMeshTextured(context, 0.5f);

            _shaderColored.Load(context);

            _shaderColored.ActionRegistry.Add<QuadColored>(e => {
                _shaderColored.SetPosition(ref e.Position);
                _shaderColored.SetSize(ref e.Size);
                _shaderColored.SetColor(ref e.Color);
            });

            _shaderTextured.Load(context);

            _shaderTextured.ActionRegistry.Add<QuadTextured>(e => {
                _shaderTextured.SetPosition(ref e.Position);
                _shaderTextured.SetSize(ref e.Size);
                _shaderTextured.SetTexture(e.TextureView);
            });
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing) {
            _items.Clear();

            Utilities.Dispose(ref _meshColored);
            Utilities.Dispose(ref _meshTextured);
            Utilities.Dispose(ref _shaderColored);
            Utilities.Dispose(ref _shaderTextured);
        }

        public void Add(QuadColored item) {
            _items.Add(item, _shaderColored, _meshColored);
        }

        public void Add(QuadTextured item) {
            _items.Add(item, _shaderTextured, _meshTextured);
        }

        public void Remove(QuadColored item) {
            _items.Remove(item, _shaderColored, _meshColored);
        }

        public void Remove(QuadTextured item) {
            _items.Remove(item, _shaderTextured, _meshTextured);
        }

        public void Render(DeviceContext context) {
            _items.Render(context);
        }

        public void Render(DeviceContext context, QuadColored quad) {
            _shaderColored.Apply(context);
            _meshColored.Apply(context);

            _shaderColored.ActionRegistry.Apply(quad);
            _shaderColored.Update(context);

            _meshColored.Render(context);
        }

        public void Render(DeviceContext context, QuadTextured quad) {
            _shaderTextured.Apply(context);
            _meshTextured.Apply(context);

            _shaderTextured.ActionRegistry.Apply(quad);
            _shaderTextured.Update(context);

            _meshTextured.Render(context);
        }
    }
}
