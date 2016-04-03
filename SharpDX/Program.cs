using SharpDX.Core.Text;
using SharpDX.Data;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Extensions;
using SharpDX.Scenes;
using SharpDX.Windows;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Device = SharpDX.Direct3D11.Device;
using Resource = SharpDX.Direct3D11.Resource;

namespace SharpDX
{
    static class Program
    {
        private const string Title = "SharpDx";

        private static RenderForm _form;
        private static Device device;
        private static SwapChain swapChain;
        private static SwapChainDescription swapChainDesc;
        private static Texture2D backBuffer, depthBuffer;
        private static RenderTargetView renderView;
        private static DepthStencilView depthView;
        private static Factory factory;
        private static FrameCounter _fps;
        private static Stopwatch clock;
        private static bool userResized;
        private static long elapsedPrevious;

        public static RenderForm Form => _form;
        public static FrameCounter Fps => _fps;
        public static int FormWidth => _form.ClientSize.Width;
        public static int FormHeight => _form.ClientSize.Height;
        private static Color Background = new Color(0, 0, 200, 0);

        private static TestScene scene = new TestScene();
        private static bool isSceneCreated;

        private static TextDevice _textDevice;
        public static TextDevice TextDevice => _textDevice;


        [STAThread]
        static void Main() {
            _form = new RenderForm();
            _form.StartPosition = FormStartPosition.CenterScreen;
            _form.Size = new Size(800, 600);
            _form.UserResized += (sender, e) => userResized = true;
            _form.KeyUp += Form_KeyUp;

            Exception fatalError = null;
            try {
#if DEBUG
                // Debugging Only! Tracks dispose references
                Configuration.EnableObjectTracking = true;
#endif

                Initialize();

                userResized = true;
                RenderLoop.Run(_form, Process);
            }
            catch (Exception error) {
                fatalError = error;
            }
            finally {
                Dispose();
            }

            if (fatalError != null) {
                var message = fatalError.UnfoldMessage();
                Debug.WriteLine($"FATAL: {message}");
                throw fatalError;
            }
        }

        private static void Initialize() {
            _fps = new FrameCounter();
            //fps.OnUpdate += v => SetTitle(v);

            swapChainDesc = new SwapChainDescription {
                BufferCount = 1,
                ModeDescription = new ModeDescription(FormWidth, FormHeight, new Rational(60, 1), Format.R8G8B8A8_UNorm),
                IsWindowed = true,
                OutputHandle = _form.Handle,
                SampleDescription = new SampleDescription(1, 0),
                SwapEffect = SwapEffect.Discard,
                Usage = Usage.RenderTargetOutput,
            };

            var deviceOptions = DeviceCreationFlags.Debug | DeviceCreationFlags.BgraSupport;
            Device.CreateWithSwapChain(DriverType.Hardware, deviceOptions, swapChainDesc, out device, out swapChain);

            _textDevice = new TextDevice();

            factory = swapChain.GetParent<Factory>();
            factory.MakeWindowAssociation(_form.Handle, WindowAssociationFlags.IgnoreAll);

            clock = new Stopwatch();
            clock.Start();
        }

        private static void Dispose() {
            Utilities.Dispose(ref _textDevice);

            scene?.Dispose();

            depthBuffer?.Dispose();
            depthView?.Dispose();
            renderView?.Dispose();
            backBuffer?.Dispose();
            device?.Dispose();
            swapChain?.Dispose();
            factory?.Dispose();
        }

        private static void Process() {
            var context = device.ImmediateContext;

            if (!isSceneCreated) {
                scene.Initialize(context);
                isSceneCreated = true;
            }

            if (userResized) {
                Resize(context);
                userResized = false;
            }

            var elapsedCurrent = clock.ElapsedMilliseconds;
            var elapsed = elapsedCurrent - elapsedPrevious;
            var time = elapsed / 1000f;
            elapsedPrevious = elapsedCurrent;
            _fps.Update(elapsed);

            scene.Update(time);

            context.ClearDepthStencilView(depthView, DepthStencilClearFlags.Depth, 1f, 0);
            context.ClearRenderTargetView(renderView, Background);

            scene.Render(context);

            _textDevice.BeginDraw();

            scene.RenderText(_textDevice);

            _textDevice.EndDraw();

            swapChain.Present(0, PresentFlags.None);
        }

        private static void Resize(DeviceContext context) {
            Utilities.Dispose(ref backBuffer);
            Utilities.Dispose(ref renderView);
            Utilities.Dispose(ref depthBuffer);
            Utilities.Dispose(ref depthView);
            _textDevice.ResizePre();

            swapChain.ResizeBuffers(swapChainDesc.BufferCount, FormWidth, FormHeight, Format.Unknown, SwapChainFlags.None);

            backBuffer = Resource.FromSwapChain<Texture2D>(swapChain, 0);
            _textDevice.ResizePost(backBuffer);

            renderView = new RenderTargetView(device, backBuffer);

            depthBuffer = new Texture2D(device, new Texture2DDescription() {
                Format = Format.D32_Float,
                ArraySize = 1,
                MipLevels = 1,
                Width = FormWidth,
                Height = FormHeight,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.DepthStencil,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None,
            });

            depthView = new DepthStencilView(device, depthBuffer);

            context.Rasterizer.SetViewport(new Viewport(0, 0, FormWidth, FormHeight, 0f, 1f));
            context.OutputMerger.SetTargets(depthView, renderView);

            scene.Resize();
        }

        private static void Form_KeyUp(object sender, KeyEventArgs e) {
            switch (e.KeyCode) {
                case Keys.F4:
                    swapChain.SetFullscreenState(false, null);
                    break;
                case Keys.F5:
                    swapChain.SetFullscreenState(true, null);
                    break;
                case Keys.Enter:
                    if (e.Alt) swapChain.IsFullScreen = !swapChain.IsFullScreen;
                    break;
                case Keys.Escape:
                    _form.Close();
                    break;
            }
        }

        //-----------------------------

        //private static void SetTitle(int? frameCount = null) {
        //    var text = Title;
        //    if (frameCount.HasValue)
        //        text += $" [{frameCount.Value}]";

        //    if (scene != null)
        //        text += $" [{scene.RenderCount}/{scene.EntityCount}]";

        //    _form.Text = text;
        //}
    }
}
