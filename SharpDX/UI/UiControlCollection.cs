using SharpDX.Core;
using SharpDX.Core.Quad;
using SharpDX.Direct2D1;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using System;
using System.Collections.Generic;

namespace SharpDX.UI
{
    class UiControlCollection : List<IUiControl>, IDisposable
    {
        private Vector2 _position, _size;
        private Texture2D _texture;
        private RenderTarget _renderText;
        private RenderTargetView _renderView;
        private QuadColored _quad;
        private QuadTextured _bufferQuad;
        private Color4 _backgroundColor;
        private int _bufferWidth, _bufferHeight;

        public bool EnableBuffering {get;}

        public bool IsLayoutValid {get; private set;}
        public bool IsRenderValid {get; private set;}

        public Vector2 ScreenPosition {get; private set;}

        public Vector2 Position {
            get {return _position;}
            set {
                _position = value;
                InvalidateLayout();
            }
        }

        public Vector2 Size {
            get {return _size;}
            set {
                _size = value;
                InvalidateLayout();
            }
        }


        public UiControlCollection(bool enableBuffering = false) {
            this.EnableBuffering = enableBuffering;

            _backgroundColor = new Color4();

            if (enableBuffering) {
                _bufferQuad = new QuadTextured();
            }
            else {
                _quad = new QuadColored();
                _quad.Color = new Color4();
            }
        }

        public void Dispose() {
            foreach (IDisposable i in this)
                i?.Dispose();

            Clear();

            Utilities.Dispose(ref _bufferQuad);
            Utilities.Dispose(ref _renderView);
            Utilities.Dispose(ref _renderText);
            Utilities.Dispose(ref _texture);
        }

        public void Update(UiUpdateEventArgs e) {
            for (int i = 0; i < Count; i++)
                this[i].Update(e);
        }

        public void UpdateLayout(Vector2 parentPosition) {
            if (!IsLayoutValid) {
                ScreenPosition = parentPosition + Position;
                IsRenderValid = false;
                IsLayoutValid = true;

                if (EnableBuffering) {
                    _bufferQuad.Size = Size;
                    _bufferQuad.Position = ScreenPosition;
                }
                else {
                    _quad.Size = Size;
                    _quad.Position = ScreenPosition;
                }
            }

            for (int i = 0; i < Count; i++)
                this[i].UpdateLayout(ScreenPosition);
        }

        public bool RenderBuffers(Context context, UiManager uiMgr) {
            var result = false;
            IUiControl control;

            for (int i = 0; i < Count; i++) {
                control = this[i];
                if (!control.IsVisible) continue;

                if (control.RenderBuffers(context, uiMgr))
                    result = true;
            }

            if (EnableBuffering && !IsRenderValid) {
                BuildBuffer(context);
                IsRenderValid = true;

                var x = (int)(ScreenPosition.X * Program.FormWidth);
                var y = (int)(ScreenPosition.Y * Program.FormHeight);
                var w = (int)(Size.X * Program.FormWidth);
                var h = (int)(Size.Y * Program.FormHeight);
                var viewport = new Viewport(x, y, w, h);

                context.Immediate.OutputMerger.SetRenderTargets(_renderView);
                context.Immediate.ClearRenderTargetView(_renderView, _backgroundColor);
                context.Immediate.Rasterizer.SetViewport(viewport);
                context.RenderTarget2D = _renderText;

                RenderControls(context, uiMgr);
                result = true;
            }

            return result;
        }

        public void Render(Context context, UiManager uiMgr) {
            if (EnableBuffering) {
                uiMgr.Render(context, _bufferQuad);
            }
            else {
                uiMgr.Render(context, _quad);
                RenderControls(context, uiMgr);
            }
        }

        private void RenderControls(Context context, UiManager uiMgr) {
            IUiControl control;

            for (int i = 0; i < Count; i++) {
                control = this[i];
                if (!control.IsVisible) continue;

                control.Render(context, uiMgr);
            }
        }

        private void BuildBuffer(Context context) {
            Utilities.Dispose(ref _texture);
            Utilities.Dispose(ref _renderView);
            Utilities.Dispose(ref _renderText);

            _bufferWidth = (int)Math.Ceiling(Size.X);
            _bufferHeight = (int)Math.Ceiling(Size.Y);

            if (_bufferWidth == 0 || _bufferHeight == 0) {
                _bufferQuad.SetTexture(context.Immediate, null);
                return;
            }

            _texture = new Texture2D(context.Device, new Texture2DDescription {
                SampleDescription = new SampleDescription(1, 0),
                Format = Format.R32G32B32A32_Float,
                BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
                Usage = ResourceUsage.Default,
                Width = _bufferWidth,
                Height = _bufferHeight,
                MipLevels = 1,
                ArraySize = 1,
            });

            using (var surface = _texture.QueryInterface<Surface>()) {
                var pixelFormat = new PixelFormat(Format.Unknown, Direct2D1.AlphaMode.Premultiplied);
                _renderText = new RenderTarget(context.Factory2D, surface, new RenderTargetProperties(pixelFormat));
            }

            _renderView = new RenderTargetView(context.Device, _texture);
            _bufferQuad.SetTexture(context.Immediate, _texture);
        }

        public void InvalidateLayout() {
            IsLayoutValid = false;
            for (var i = 0; i < Count; i++)
                this[i].InvalidateLayout();
        }

        public void InvalidateRender() {
            IsRenderValid = false;
            // Invoke event upwards
        }

        public void SetBackgroundColor(ref Color4 color) {
            _backgroundColor = color;

            if (!EnableBuffering)
                _quad.Color = color;
        }
    }
}
