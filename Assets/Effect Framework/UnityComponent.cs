using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public abstract class UnityComponent : MonoBehaviour
{
    protected void UseEffect<TInstance, TDependency>(TInstance instance, string propertyToTrackName, Action callback)
        where TInstance : class
        where TDependency : struct
    {
        var effect = new EffectDep<TInstance, TDependency>(instance, propertyToTrackName, callback);
        effects.Add(effect);
    }

    private interface IEffect
    {
        bool HasChanged();
        void Reset();
        void InvokeCallback();
    }

    private List<IEffect> effects = new List<IEffect>();
    
    private class EffectDep<TInstance, TDependencyType> : IEffect
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

    public abstract class RenderAffector<T>
    {
        public T _Value { get; private set; }
        private T OldValue { get; set; }
        private Action _rerender;

        protected RenderAffector(Action rerender, T initialValue)
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
    
    protected class State<T> : RenderAffector<T>
    {
        public T Value => _Value;

        public State(Action rerender, T initialValue) : base(rerender, initialValue)
        {
            
        }

        public void SetState(T newState)
        {
            SetValue(newState);
        }
    }

    protected class Prop<T> : RenderAffector<T> where T : struct
    {
        public T Value
        {
            set => SetValue(value);
            get => base._Value;
        }

        public Prop(Action rerender, T initialValue) : base(rerender, initialValue)
        {
            
        }
    }
    
    protected State<T> UseState<T>(T e) where T : struct
    {
        return new State<T>(ScheduleRerender, e);
    }

    /// <summary>
    /// Props are value types so they will only invoke rerender when a value actually changes
    /// </summary>
    /// <param name="e"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    protected Prop<T> InitProp<T>(T e) where T : struct
    {
        return new Prop<T>(ScheduleRerender, e);
    }
    
    private bool isInit = false;

    [SerializeField] protected UnityComponent[] children;
    private UnityComponentManager _manager;    
    
    public void InitializeComponent(UnityComponentManager manager)
    {
        if (isInit)
        {
            return;
        }
        
        _manager = manager;
        _manager.RegisterComponent(this);
        
        OnMount();
        
        children.ToList().ForEach((c) => c.InitializeComponent(manager));
        
        Rerender();
        
        isInit = true;
    }

    bool shouldRerender;
    
    void ScheduleRerender()
    {
        shouldRerender = true;
    }

    public void Run()
    {
        if (effects != null)
        {
            foreach (var effect in effects)
            {
                if (effect.HasChanged())
                {
                    effect.Reset();
                    effect.InvokeCallback();
                }
            }
        }

        if (shouldRerender)
        {
            Rerender();
            shouldRerender = false;
        }
    }

    /// <summary>
    /// Use this to initialize State  variables
    /// </summary>
    protected abstract void OnMount();

    /// <summary>
    /// Will be called only when any state instance changes
    /// </summary>
    protected abstract void Rerender();
    
    
}