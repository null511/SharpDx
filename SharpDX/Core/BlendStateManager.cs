using SharpDX.Direct3D11;

namespace SharpDX.Core
{
    class BlendStateManager : StateManager<BlendState, BlendStateDescription>
    {
        private const string keyDefault = "default";
        private const string keyText = "text";
        private const string keyQuad = "quad";


        public BlendStateManager() : base(() => BlendStateDescription.Default(), OnBuild) {}

        private static BlendState OnBuild(DeviceContext context, ref BlendStateDescription description) {
            return new BlendState(context.Device, description);
        }

        //=============================

        public BlendState Default(DeviceContext context) {
            return Get(context, keyDefault, (ref BlendStateDescription description) => {});
        }

        public BlendState Text(DeviceContext context) {
            return Get(context, keyText, (ref BlendStateDescription description) => {
                description.RenderTarget[0] = new RenderTargetBlendDescription(
                    true,
                    BlendOption.SourceAlpha,
                    BlendOption.InverseSourceAlpha,
                    BlendOperation.Add,
                    BlendOption.One,
                    BlendOption.Zero,
                    BlendOperation.Add,
                    ColorWriteMaskFlags.All);
            });
        }

        public BlendState Quad(DeviceContext context) {
            return Get(context, keyQuad, (ref BlendStateDescription description) => {
                description.RenderTarget[0] = new RenderTargetBlendDescription(
                    true,
                    BlendOption.SourceAlpha,
                    BlendOption.InverseSourceAlpha,
                    BlendOperation.Add,
                    BlendOption.One,
                    BlendOption.Zero,
                    BlendOperation.Add,
                    ColorWriteMaskFlags.All);
            });
        }
    }
}
