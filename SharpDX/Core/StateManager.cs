using SharpDX.Direct3D11;
using SharpDX.Extensions;
using System;
using System.Collections.Generic;

namespace SharpDX.Core
{
    abstract class StateManager<TState, TDescription> : IDisposable
        where TState : IDisposable
        where TDescription : struct
    {
        public delegate void CreateEvent(ref TDescription description);
        public delegate TState BuildStateEvent(DeviceContext context, ref TDescription description);

        private Dictionary<string, TState> _states;
        private Func<TDescription> _buildDescription;
        private BuildStateEvent _buildState;


        public StateManager(Func<TDescription> buildDescription, BuildStateEvent buildState) {
            this._buildDescription = buildDescription;
            this._buildState = buildState;

            _states = new Dictionary<string, TState>();
        }

        ~StateManager() {
            Dispose(false);
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(true);
        }

        protected virtual void Dispose(bool disposing) {
            foreach (IDisposable i in _states.Values)
                i.Dispose();

            _states.Clear();
        }

        protected TState Get(DeviceContext context, string key, CreateEvent createEvent) {
            TState state;
            if (_states.TryGetValue(key, out state)) return state;

            var description = _buildDescription();
            createEvent.Invoke(ref description);
            state = _buildState(context, ref description);
            _states.Add(key, state);
            return state;
        }
    }
}
