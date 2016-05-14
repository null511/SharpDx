using SharpDX.Direct3D11;

namespace SharpDX.Core
{
    class DepthStateManager : StateManager<DepthStencilState, DepthStencilStateDescription>
    {
        private const string keyDefault = "default";
        private const string keyText = "text";
        private const string keyQuad = "quad";


        public DepthStateManager() : base(() => DepthStencilStateDescription.Default(), OnBuild) {}

        private static DepthStencilState OnBuild(DeviceContext context, ref DepthStencilStateDescription description) {
            return new DepthStencilState(context.Device, description);
        }

        //=============================

        public DepthStencilState Default(DeviceContext context) {
            return Get(context, keyDefault, (ref DepthStencilStateDescription description) => {});
        }

        public DepthStencilState Text(DeviceContext context) {
            return Get(context, keyText, (ref DepthStencilStateDescription description) => {
                description.IsDepthEnabled = false;
            });
        }

        public DepthStencilState Quad(DeviceContext context) {
            return Get(context, keyQuad, (ref DepthStencilStateDescription description) => {
                description.IsDepthEnabled = true;
                description.DepthComparison = Comparison.LessEqual;
            });
        }
    }
}
