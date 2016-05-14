using SharpDX.Core;

namespace SharpDX.UI
{
    interface IUiBufferedControl : IUiControl
    {
        void RenderToBuffer(Context context);
    }
}
