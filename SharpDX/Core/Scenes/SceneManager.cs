using SharpDX.Windows;
using System;
using System.Collections.Generic;

namespace SharpDX.Core.Scenes
{
    class SceneManager : IDisposable
    {
        private readonly Dictionary<Type, IScene> scenes;
        private Action<Context> _pendingLoad;
        private IScene _current;
        private bool _isFirstFrame;


        public SceneManager() {
            scenes = new Dictionary<Type, IScene>();
        }

        ~SceneManager() {
            Dispose(false);
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing) {
            foreach (var scene in scenes.Values)
                scene.Dispose();

            scenes.Clear();
        }

        public void Load<T>()
            where T : IScene {
            _pendingLoad = c => loadAction<T>(c, true);
        }

        public void LoadImmediate<T>(Context context)
            where T : IScene {
            loadAction<T>(context, false);
        }

        private void loadAction<T>(Context context, bool resize)
            where T : IScene {
            _current?.Unload();

            var scene = Find<T>();
            if (scene == null)
                throw new ApplicationException($"Scene '{typeof(T).Name}' not found!");

            _current = scene;
            _current.Load(context);
            if (resize) _current.Resize();
        }

        public void Add<T>(T scene)
            where T : IScene
        {
            scenes.Add(typeof(T), scene);
        }

        public void Update(float time) {
            _current?.Update(time);
        }

        public void Resize() {
            _current?.Resize();
        }

        public void Render(Context context) {
            if (_pendingLoad != null) {
                _pendingLoad(context);
                _pendingLoad = null;
                _isFirstFrame = true;
                return;
            }

            if (_isFirstFrame) {
                context.Restore();
                _isFirstFrame = false;
            }

            _current?.Render(context);
        }

        private T Find<T>()
            where T : IScene
        {
            var key = typeof(T);

            IScene scene;
            if (scenes.TryGetValue(key, out scene)) return (T)scene;
            return default(T);
        }
    }
}
