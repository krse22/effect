using System;
using UnityEngine;

namespace EffectFramework.Rerender
{
    /// <summary>
    /// Causes rerender when value is changed, used to pass props from parent to children
    /// </summary>
    /// <typeparam name="T">Value type of param</typeparam>
    public class Prop<T> : AffectRerender<T> where T : struct
    {
        /// <summary>
        /// Setting the value here will cause rerender
        /// </summary>
        public T Value
        {
            set => SetValue(value);
            get => base._Value;
        }

        internal Prop(Action rerender, T initialValue) : base(rerender, initialValue) {}
    }
}

