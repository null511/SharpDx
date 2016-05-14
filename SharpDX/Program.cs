using SharpDX.Core;
using SharpDX.Core.Scenes;
using SharpDX.Data;
using SharpDX.Diagnostics;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Extensions;
using SharpDX.Scenes;
using SharpDX.Windows;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Device = SharpDX.Direct3D11.Device;

namespace SharpDX
{
    static class Program
    {
        private const string Title = "SharpDx";

        private static Context _context;
        private static RenderForm _form;
        private static SwapChain swapChain;
        private static SwapChainDescription swapChainDesc;
        private static Factory factory;
        private static FrameCounter _fps;
        private static Stopwatch clock;
        private static bool userResized;
        private static long elapsedPrevious;
        private static SceneManager _sceneMgr;
        private static bool isSceneCreated;

        public static RenderForm Form => _form;
        public static FrameCounter Fps => _fps;
        public static int FormWidth => _form.ClientSize.Width;
        public static int FormHeight => _form.ClientSize.Height;
        private static Color Background = new Color(0, 0, 0, 0);


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
                // THIS IS INCREDIBLY SLOW WITH OLDER DEBUGGER
                //Configuration.EnableObjectTracking = true;
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
                GC.Collect();
            }

            var active = ObjectTracker.FindActiveObjects();
            var unreleased = ObjectTracker.ReportActiveObjects();
            Console.WriteLine("\n==========================");
            Console.WriteLine("Active Objects\n");
            Console.WriteLine(unreleased);
            if (active.Any())
                Console.WriteLine(string.Join(",", active));
            Console.WriteLine();

            if (fatalError != null) {
                var message = fatalError.UnfoldMessage();
                Debug.WriteLine($"FATAL: {message}");
                throw fatalError;
            }
        }

        private static void Initialize() {
            _fps = new FrameCounter();

            _context = new Context();

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
            Device.CreateWithSwapChain(DriverType.Hardware, deviceOptions, swapChainDesc, out _context.Device, out swapChain);

            factory = swapChain.GetParent<Factory>();
            factory.MakeWindowAssociation(_form.Handle, WindowAssociationFlags.IgnoreAll);

            _sceneMgr = new SceneManager();
            _sceneMgr.Add(new MainMenuScene());
            _sceneMgr.Add(new LoadScene());
            _sceneMgr.Add(new TestScene());

            clock = new Stopwatch();
            clock.Start();
        }

        private static void Dispose() {
            Utilities.Dispose(ref _sceneMgr);
            Utilities.Dispose(ref swapChain);
            Utilities.Dispose(ref factory);
            Utilities.Dispose(ref _form);
            Utilities.Dispose(ref _context);
        }

        private static void Process() {
            if (!isSceneCreated) {
                _sceneMgr.LoadImmediate<MainMenuScene>(_context);
                isSceneCreated = true;
            }

            if (userResized) {
                Resize();
                userResized = false;
            }

            var elapsedCurrent = clock.ElapsedMilliseconds;
            var elapsed = elapsedCurrent - elapsedPrevious;
            var time = elapsed / 1000f;
            elapsedPrevious = elapsedCurrent;
            _fps.Update(elapsed);

            _sceneMgr.Update(time);

            _sceneMgr.Render(_context);

            swapChain.Present(0, PresentFlags.None);
        }

        private static void Resize() {
            _context.Resize(swapChain, swapChainDesc.BufferCount, FormWidth, FormHeight);

            _sceneMgr.Resize();
        }

        public static void GoTo<T>()
            where T : IScene
        {
            _sceneMgr.Load<T>();
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
            }
        }
    }
}
