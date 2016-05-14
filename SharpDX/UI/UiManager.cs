using SharpDX.Core;
using SharpDX.Core.Quad;
using System;

namespace SharpDX.UI
{
    class UiManager : IDisposable
    {
        public UiControlCollection Controls {get;}

        private QuadManager _quadMgr;


        public UiManager() {
            Controls = new UiControlCollection();
            _quadMgr = new QuadManager();
        }

        ~UiManager() {
            Dispose(false);
        }

        public void Initialize(Context context) {
            _quadMgr.Initialize(context.Immediate);
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing) {
            Controls.Dispose();

            Utilities.Dispose(ref _quadMgr);
        }

        public void Add(IUiControl control) {
            Controls.Add(control);
        }

        public void Remove(IUiControl control) {
            Controls.Remove(control);
        }

        public void Update(UiUpdateEventArgs e) {
            Controls.Update(e);
        }

        public bool RenderBuffers(Context context) {
            Controls.UpdateLayout(Vector2.Zero);

            return Controls.RenderBuffers(context, this);
        }

        public void Render(Context context) {
            Controls.Render(context, this);
        }

        public void Render(Context context, QuadColored quad) {
            _quadMgr.Render(context.Immediate, quad);
        }

        public void Render(Context context, QuadTextured quad) {
            _quadMgr.Render(context.Immediate, quad);
        }
    }
}
