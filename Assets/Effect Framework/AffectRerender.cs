using System;
using System.Collections.Generic;
using UnityEngine;

namespace EffectFramework.Rerender
{
    public class AffectRerender<T> : MonoBehaviour
    {
        public T _Value { get; private set; }
        private T OldValue { get; set; }
        private Action _rerender;

        internal AffectRerender(Action rerender, T initialValue)
        {
            _Value = initialValue;
            OldValue = initialValue;
            _rerender = rerender;
        }

        protected void SetValue(T _newState)
        {
            _Value = _newState;
            if (HasChanged())
            {
                _rerender();
                UpdateState();
            }
        }

        bool HasChanged()
        {
            return !EqualityComparer<T>.Default.Equals(_Value, OldValue); // Reflection based for reference types
        }

        void UpdateState()
        {
            OldValue = _Value;
        }
    }
}

