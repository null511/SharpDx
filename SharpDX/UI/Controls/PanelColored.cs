using SharpDX.Core;
using SharpDX.Core.Quad;

namespace SharpDX.UI.Controls
{
    class PanelColored : ContainerBase
    {
        private QuadColored _quad;

        public Color4 Color {
            get {return _quad.Color;}
            set {
                _quad.Color = value;
                InvalidateRender();
            }
        }


        public PanelColored() {
            _quad = new QuadColored();
        }

        public override void UpdateLayout(Vector2 parentPosition) {
            base.UpdateLayout(parentPosition);

            _quad.Position = ScreenPosition;
            _quad.Size = Size;
        }

        public override void Render(Context context, UiManager uiMgr) {
            uiMgr.Render(context, _quad);

            base.Render(context, uiMgr);
        }

        public PanelColored SetColor(Color4 color) {
            this.Color = color;
            return this;
        }
    }
}
