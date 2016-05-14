using SharpDX.Core;

namespace SharpDX.UI
{
    interface IUiControl
    {
        event InvalidationEvent Invalidated;

        bool IsEnabled {get; set;}
        bool IsVisible {get; set;}
        Vector2 Position {get; set;}
        Vector2 Size {get; set;}

        void Update(UiUpdateEventArgs e);
        void UpdateLayout(Vector2 parentPosition);
        void Render(Context context, UiManager uiMgr);
        bool RenderBuffers(Context context, UiManager uiMgr);
        void InvalidateLayout();
        void InvalidateRender();
    }

    static class UiControlUtils
    {
        public static T SetPosition<T>(this T control, int x, int y)
            where T : IUiControl
        {
            control.Position = new Vector2(x, y);
            return control;
        }

        public static T SetSize<T>(this T control, int width, int height)
            where T : IUiControl
        {
            control.Size = new Vector2(width, height);
            return control;
        }

        public static T SetScreenPosition<T>(this T control, int x, int y)
            where T : IUiControl
        {
            control.Position = new Vector2(x / (float)Program.FormWidth, y / (float)Program.FormHeight);
            return control;
        }

        public static T SetScreenSize<T>(this T control, int width, int height)
            where T : IUiControl {
            control.Size = new Vector2(width / (float)Program.FormWidth, height / (float)Program.FormHeight);
            return control;
        }
    }
}
