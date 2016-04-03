using System;
using System.Collections.Generic;

namespace SharpDX.Core.Shaders
{
    class ShaderActionRegistry
    {
        private IDictionary<Type, Action<IObject>> _actions;


        public ShaderActionRegistry() {
            _actions = new Dictionary<Type, Action<IObject>>();
        }

        public void Add<TObject>(Action<TObject> action)
            where TObject : IObject {
            _actions.Add(typeof(TObject), i => action((TObject)i));
        }

        public void Clear() {
            _actions.Clear();
        }

        public Action<IObject> GetAction(Type type)
        {
            Action<IObject> action;
            if (_actions.TryGetValue(type, out action)) return action;
            return null;
        }

        public void Apply(IObject entity)
        {
            var type = entity.GetType();
            var action = GetAction(type);
            if (action == null) throw new ApplicationException($"No action found for type '{type.Name}'!");
            action.Invoke(entity);
        }

        public void Apply<TObject>(IObject entity)
        {
            var type = typeof(TObject);
            var action = GetAction(type);
            if (action == null) throw new ApplicationException($"No action found for type '{type.Name}'!");
            action.Invoke(entity);
        }
    }
}
