

using System;
using System.Collections.Generic;
using System.Reflection;

namespace EffectFramework.Rerender
{
   internal class EffectDep<TInstance, TDependencyType> : IEffect
        where TInstance : class
        where TDependencyType : struct
    {
        private readonly TInstance _instance;
        private TDependencyType _oldValue;

        private readonly FieldInfo _field;
        private readonly PropertyInfo _property;

        private Action _callback;

        public EffectDep(TInstance instance, string track, Action callback)
        {
            _instance = instance;
            _callback = callback;

            var bidingFlags = BindingFlags.Instance | BindingFlags.Public;
            var field = typeof(TInstance).GetField(track, bidingFlags);
            if (field != null)
            {
                _field = field;
                _oldValue = GetFieldValue();
                return;
            }

            var property = typeof(TInstance).GetProperty(track, bidingFlags);
            if (property != null)
            {
                _property = property;
                _oldValue = GetPropertyValue();
            }
        }

        private TDependencyType GetFieldValue()
        {
            return (TDependencyType)_field.GetValue(_instance);
        }
        
        private TDependencyType GetPropertyValue()
        {
            return (TDependencyType)_property.GetValue(_instance);
        }

        public bool HasChanged()
        {
            if (_field != null)
            {
                return !EqualityComparer<TDependencyType>.Default.Equals(_oldValue, GetFieldValue());
            }
            return !EqualityComparer<TDependencyType>.Default.Equals(_oldValue, GetPropertyValue());
        }

        public void Reset()
        {
            if (_field != null)
            {
                _oldValue = GetFieldValue();
                return;
            }
            _oldValue = GetPropertyValue();
        }

        public void InvokeCallback()
        {
            _callback.Invoke();
        }
    }
}