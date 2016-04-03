using SharpDX.Direct2D1;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using System;

namespace SharpDX.Core.Text
{
    class TextDevice : IDisposable
    {
        public Direct2D1.Factory Factory2D;
        public DirectWrite.Factory FactoryDWrite;
        public RenderTarget RenderTarget2D;


        public TextDevice() {
            Factory2D = new Direct2D1.Factory();
            FactoryDWrite = new DirectWrite.Factory();
        }

        public void Dispose() {
            Factory2D?.Dispose();
            FactoryDWrite?.Dispose();
            RenderTarget2D?.Dispose();
        }

        public void ResizePre() {
            Utilities.Dispose(ref RenderTarget2D);
        }

        public void ResizePost(Texture2D backBuffer) {
            using (var surface = backBuffer.QueryInterface<Surface>()) {
                var pixelFormat = new PixelFormat(Format.Unknown, Direct2D1.AlphaMode.Premultiplied);
                RenderTarget2D = new RenderTarget(Factory2D, surface, new RenderTargetProperties(pixelFormat));
            }
        }

        public void BeginDraw() {
            RenderTarget2D.TextAntialiasMode = TextAntialiasMode.Cleartype;
            RenderTarget2D.BeginDraw();
        }

        public void EndDraw() {
            RenderTarget2D.EndDraw();
            RenderTarget2D.AntialiasMode = AntialiasMode.PerPrimitive;
        }

        public Brush GetBrush(float r, float g, float b, float a) {
            return new SolidColorBrush(RenderTarget2D, new RawColor4(r, g, b, a));
        }

        public Brush GetBrush(ref Color4 color) {
            var rawColor = new RawColor4(color.Red, color.Green, color.Blue, color.Alpha);
            return new SolidColorBrush(RenderTarget2D, rawColor);
        }
    }
}
