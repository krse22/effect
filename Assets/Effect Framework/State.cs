using System;

namespace EffectFramework.Rerender
{
    /// <summary>
    /// Causes rerender when value is changed, used as components state
    /// private State<int> _score;
    /// </summary>
    /// <typeparam name="T">Value type of param</typeparam>
    public class State<T> : AffectRerender<T>
    {
        public T Value => _Value;

        internal State(Action rerender, T initialValue) : base(rerender, initialValue) {}

        public void SetState(T newState)
        {
            SetValue(newState);
        }
    }
}
