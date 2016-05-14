using System;

namespace SharpDX.Core.Scenes
{
    interface IScene : IDisposable
    {
        Input Input { get; }

        void Load(Context context);
        void Unload();
        void Update(float time);
        void Resize();
        void Render(Context context);
    }
}
