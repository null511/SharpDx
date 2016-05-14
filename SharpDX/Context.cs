using SharpDX.Direct2D1;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using System;

namespace SharpDX.Core
{
    class Context : IDisposable
    {
        public DirectInput.DirectInput InputDevice;
        public Direct3D11.Device Device;
        public Direct2D1.Factory Factory2D;
        public DirectWrite.Factory FactoryDWrite;
        private RenderTarget backBufferText;
        private Texture2D backBuffer, depthBuffer;
        private RenderTargetView renderView;
        private DepthStencilView depthView;

        public RenderTarget RenderTarget2D {get; set;}
        public Direct3D11.DeviceContext Immediate => Device.ImmediateContext;


        public Context() {
            InputDevice = new DirectInput.DirectInput();
            Factory2D = new Direct2D1.Factory();
            FactoryDWrite = new DirectWrite.Factory();
        }

        public void Dispose() {
            Utilities.Dispose(ref depthView);
            Utilities.Dispose(ref renderView);
            Utilities.Dispose(ref depthBuffer);
            Utilities.Dispose(ref backBufferText);
            Utilities.Dispose(ref backBuffer);
            Utilities.Dispose(ref Factory2D);
            Utilities.Dispose(ref FactoryDWrite);
            Utilities.Dispose(ref InputDevice);
            Utilities.Dispose(ref Device);
        }

        public void Resize(SwapChain swapChain, int bufferCount, int width, int height) {
            Utilities.Dispose(ref backBufferText);
            Utilities.Dispose(ref backBuffer);
            Utilities.Dispose(ref depthBuffer);
            Utilities.Dispose(ref renderView);
            Utilities.Dispose(ref depthView);

            if (width == 0 || height == 0) return;

            swapChain.ResizeBuffers(bufferCount, width, height, Format.Unknown, SwapChainFlags.None);

            backBuffer = Direct3D11.Resource.FromSwapChain<Texture2D>(swapChain, 0);

            using (var surface = backBuffer.QueryInterface<Surface>()) {
                var pixelFormat = new PixelFormat(Format.Unknown, Direct2D1.AlphaMode.Premultiplied);
                backBufferText = new RenderTarget(Factory2D, surface, new RenderTargetProperties(pixelFormat));
            }

            renderView = new RenderTargetView(Device, backBuffer);

            depthBuffer = new Texture2D(Device, new Texture2DDescription() {
                Format = Format.D32_Float,
                ArraySize = 1,
                MipLevels = 1,
                Width = width,
                Height = height,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.DepthStencil,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None,
            });

            depthView = new DepthStencilView(Device, depthBuffer);

            Restore();
        }

        public void Restore() {
            var i = Device.ImmediateContext;
            i.Rasterizer.SetViewport(new Viewport(0, 0, Program.FormWidth, Program.FormHeight, 0f, 1f));
            i.OutputMerger.SetTargets(depthView, renderView);
            RenderTarget2D = backBufferText;
        }

        public void BeginText() {
            RenderTarget2D.TextAntialiasMode = TextAntialiasMode.Cleartype;
            RenderTarget2D.BeginDraw();
        }

        public void EndText() {
            RenderTarget2D.EndDraw();
            RenderTarget2D.AntialiasMode = AntialiasMode.PerPrimitive;
        }

        public void Clear(ref Color4 color) {
            Device.ImmediateContext.ClearRenderTargetView(renderView, color);
        }

        public void ClearDepth(float depth) {
            Device.ImmediateContext.ClearDepthStencilView(depthView, DepthStencilClearFlags.Depth, depth, 0);
        }
    }

    static class ContextExtensions
    {
        public static Brush GetBrush(this Context context, float r, float g, float b, float a) {
            return new SolidColorBrush(context.RenderTarget2D, new RawColor4(r, g, b, a));
        }

        public static Brush GetBrush(this Context context, ref Color4 color) {
            var rawColor = new RawColor4(color.Red, color.Green, color.Blue, color.Alpha);
            return new SolidColorBrush(context.RenderTarget2D, rawColor);
        }
    }
}
