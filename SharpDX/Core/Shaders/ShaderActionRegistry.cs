using SharpDX.Core.Entities;
using System;
using System.Collections.Generic;

namespace SharpDX.Core.Shaders
{
    class ShaderActionRegistry
    {
        private IDictionary<Type, Action<Entity>> _actions;


        public ShaderActionRegistry() {
            _actions = new Dictionary<Type, Action<Entity>>();
        }

        public void Add<TEntity>(Action<TEntity> action)
            where TEntity : Entity
        {
            _actions.Add(typeof(TEntity), e => action((TEntity)e));
        }

        public void Clear() {
            _actions.Clear();
        }

        public Action<Entity> GetAction(Type type)
        {
            Action<Entity> action;
            if (_actions.TryGetValue(type, out action)) return action;
            return null;
        }

        public void Apply<TEntity>(TEntity entity)
            where TEntity : Entity
        {
            var type = entity.GetType();
            var action = GetAction(type);
            if (action == null) throw new ApplicationException($"No action found for entity type '{type.Name}'!");
            action.Invoke(entity);
        }
    }
}
