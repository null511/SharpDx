using System;

namespace SharpDX.Core.Scenes
{
    abstract class SceneBase : IScene
    {
        public Input Input { get; }


        public SceneBase() {
            Input = new Input();
        }

        ~SceneBase() {
            Dispose(false);
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(true);
        }

        protected void Dispose(bool disposing) {
            Input.Dispose();
            Unload();
        }

        public abstract void Load(Context context);
        public abstract void Unload();
        public virtual void Update(float time) {}
        public virtual void Resize() {}
        public abstract void Render(Context context);
    }
}
