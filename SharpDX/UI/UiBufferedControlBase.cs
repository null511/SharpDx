using SharpDX.Core;
using SharpDX.Core.Quad;
using SharpDX.Direct3D11;
using SharpDX.Mathematics.Interop;
using System;

namespace SharpDX.UI
{
    abstract class UiBufferedControlBase : UiControlBase
    {
        private Texture2D _texture;
        private RenderTargetView _renderView;
        private int _bufferWidth, _bufferHeight;
        private bool _isBufferValid;

        public QuadTextured Quad;
        public RawColor4 BackgroundColor;

        public bool IsBufferValid => _isBufferValid;


        public UiBufferedControlBase() {
            Quad = new QuadTextured();
        }

        public void Initialize(Context context) {
            BackgroundColor = new RawColor4(0f, 0f, 0f, 0f);
            BuildBuffer(context);
        }

        protected override void Dispose(bool disposing) {
            Utilities.Dispose(ref _renderView);
            Utilities.Dispose(ref Quad);
            Utilities.Dispose(ref _texture);

            base.Dispose(disposing);
        }

        public virtual void RenderBuffer(Context context) {
            BuildBuffer(context);
            _isBufferValid = true;

            context.Immediate.OutputMerger.SetRenderTargets(_renderView);
            context.Immediate.ClearRenderTargetView(_renderView, BackgroundColor);
        }

        private void BuildBuffer(Context context) {
            _bufferWidth = (int)Math.Ceiling(Size.X);
            _bufferHeight = (int)Math.Ceiling(Size.Y);

            Utilities.Dispose(ref _texture);
            _texture = new Texture2D(context.Device, new Texture2DDescription {
                SampleDescription = new DXGI.SampleDescription(1, 0),
                Format = DXGI.Format.R32G32B32A32_Float,
                BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
                Usage = ResourceUsage.Default,
                Width = _bufferWidth,
                Height = _bufferHeight,
                MipLevels = 1,
                ArraySize = 1,
            });

            Utilities.Dispose(ref _renderView);
            _renderView = new RenderTargetView(context.Device, _texture);
            Quad.SetTexture(context.Immediate, _texture);
        }

        public void Invalidate() {
            _isBufferValid = false;
        }
    }

    static class UiExtensions
    {
        public static T SetBackgroundColor<T>(this T control, float r, float g, float b, float a)
            where T : UiBufferedControlBase
        {
            control.BackgroundColor = new RawColor4(r, g, b, a);
            return control;
        }
    }
}
