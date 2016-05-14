using SharpDX.Core;

namespace SharpDX.UI.Controls
{
    class ContainerBase : UiControlBase
    {
        public UiControlCollection Children {get;}


        public ContainerBase(bool enableBuffering = false) {
            Children = new UiControlCollection(enableBuffering);
        }

        protected override void Dispose(bool disposing) {
            Children.Dispose();

            base.Dispose(disposing);
        }

        public override void Update(UiUpdateEventArgs e) {
            base.Update(e);

            Children.Update(e);
        }

        public override void UpdateLayout(Vector2 parentPosition) {
            base.UpdateLayout(parentPosition);

            Children.Size = Size;
            Children.UpdateLayout(ScreenPosition);
        }

        public override bool RenderBuffers(Context context, UiManager uiMgr) {
            if (IsRenderValid) return false;

            IsRenderValid = true;
            return Children.RenderBuffers(context, uiMgr);
        }

        public override void Render(Context context, UiManager uiMgr) {
            Children.Render(context, uiMgr);
        }
    }

    static class ContainerBaseUtils
    {
        public static T SetBackgroundColor<T>(this T control, Color4 color)
            where T : ContainerBase
        {
            control.Children.SetBackgroundColor(ref color);
            return control;
        }
    }
}
