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

        private static RenderForm form;
        private static Device device;
        private static SwapChain swapChain;
        private static SwapChainDescription swapChainDesc;
        private static Texture2D backBuffer, depthBuffer;
        private static RenderTargetView renderView;
        private static DepthStencilView depthView;
        private static Factory factory;
        private static FrameCounter fps;
        private static Stopwatch clock;
        private static bool userResized;
        private static long elapsedPrevious;

        public static RenderForm Form => form;
        public static int FormWidth => form.ClientSize.Width;
        public static int FormHeight => form.ClientSize.Height;
        private static Color Background = new Color(0, 0, 200, 0);

        private static TestScene scene = new TestScene();
        private static bool isSceneCreated;


        [STAThread]
        static void Main() {
            form = new RenderForm();
            form.StartPosition = FormStartPosition.CenterScreen;
            form.Size = new Size(800, 600);
            form.UserResized += (sender, e) => userResized = true;
            form.KeyUp += Form_KeyUp;

            SetTitle();

            Exception fatalError = null;
            try {
#if DEBUG
                // Debugging Only! Tracks dispose references
                Configuration.EnableObjectTracking = true;
#endif

                Initialize();

                userResized = true;
                RenderLoop.Run(form, Process);
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
            fps = new FrameCounter();
            fps.OnUpdate += v => SetTitle(v);

            swapChainDesc = new SwapChainDescription() {
                BufferCount = 1,
                ModeDescription = new ModeDescription(FormWidth, FormHeight, new Rational(60, 1), Format.R8G8B8A8_UNorm),
                IsWindowed = true,
                OutputHandle = form.Handle,
                SampleDescription = new SampleDescription(1, 0),
                SwapEffect = SwapEffect.Discard,
                Usage = Usage.RenderTargetOutput,
            };

            Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.None, swapChainDesc, out device, out swapChain);

            factory = swapChain.GetParent<Factory>();
            factory.MakeWindowAssociation(form.Handle, WindowAssociationFlags.IgnoreAll);

            clock = new Stopwatch();
            clock.Start();
        }

        private static void Dispose() {
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

            if (userResized) {
                Resize(context);
                userResized = false;
            }

            if (!isSceneCreated) {
                scene.Create(context);
                isSceneCreated = true;
            }

            var elapsedCurrent = clock.ElapsedMilliseconds;
            var elapsed = elapsedCurrent - elapsedPrevious;
            var time = elapsed / 1000f;
            elapsedPrevious = elapsedCurrent;
            fps.Update(elapsed);

            scene.Update(time);

            context.ClearDepthStencilView(depthView, DepthStencilClearFlags.Depth, 1f, 0);
            context.ClearRenderTargetView(renderView, Background);

            scene.Render(context);

            swapChain.Present(0, PresentFlags.None);
        }

        private static void Resize(DeviceContext context) {
            Utilities.Dispose(ref backBuffer);
            Utilities.Dispose(ref renderView);
            Utilities.Dispose(ref depthBuffer);
            Utilities.Dispose(ref depthView);

            swapChain.ResizeBuffers(swapChainDesc.BufferCount, FormWidth, FormHeight, Format.Unknown, SwapChainFlags.None);

            backBuffer = Resource.FromSwapChain<Texture2D>(swapChain, 0);

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
                    form.Close();
                    break;
            }
        }

        //-----------------------------

        private static void SetTitle(int? frameCount = null) {
            var text = Title;
            if (frameCount.HasValue)
                text += $" [{frameCount.Value}]";

            if (scene != null)
                text += $" [{scene.RenderCount}/{scene.EntityCount}]";

            form.Text = text;
        }
    }
}
