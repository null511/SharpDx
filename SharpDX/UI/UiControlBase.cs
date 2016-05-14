using SharpDX.Core;
using System;

namespace SharpDX.UI
{
    public delegate void IsValidChangedEvent(object sender, EventArgs e);

    abstract class UiControlBase : IUiControl, IDisposable
    {
        public event InvalidationEvent Invalidated;

        private Vector2 _position;
        private Vector2 _size;
        private bool _isLayoutValid;
        private bool _isRenderValid;

        public virtual bool IsEnabled {get; set;}
        public virtual bool IsVisible {get; set;}
        public virtual Vector2 ScreenPosition {get; private set;}

        public virtual Vector2 Position {
            get {return _position;}
            set {
                _position = value;
                IsLayoutValid = false;
                InvokeInvalidated();
            }
        }

        public virtual Vector2 Size {
            get {return _size;}
            set {
                _size = value;
                IsLayoutValid = false;
                InvokeInvalidated();
            }
        }

        public virtual bool IsLayoutValid {
            get {return _isLayoutValid;}
            set {
                if (_isLayoutValid == value) return;
                _isLayoutValid = value;
                //InvokeIsValidChanged();
            }
        }

        public virtual bool IsRenderValid {
            get {return _isRenderValid;}
            set {
                if (_isRenderValid == value) return;
                _isRenderValid = value;
                //InvokeIsValidChanged();
            }
        }


        public UiControlBase() {
            IsEnabled = true;
            IsVisible = true;
        }

        ~UiControlBase() {
            Dispose(false);
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {}

        public virtual void Update(UiUpdateEventArgs e) {}

        public virtual void UpdateLayout(Vector2 parentPosition) {
            if (!IsLayoutValid) {
                ScreenPosition = parentPosition + Position;
                IsLayoutValid = true;
            }
        }

        public virtual void Render(Context context, UiManager uiMgr) {}

        public virtual bool RenderBuffers(Context context, UiManager uiMgr) {
            //...

            return false;
        }

        public virtual void InvalidateLayout() {
            IsLayoutValid = false;
        }

        public virtual void InvalidateRender() {
            IsRenderValid = false;
        }

        protected void InvokeInvalidated() {
            Invalidated?.Invoke(this);
        }
    }
}
