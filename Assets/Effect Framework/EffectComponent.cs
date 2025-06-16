using System;
using System.Collections.Generic;
using System.Linq;
using EffectFramework.Rerender;
using UnityEngine;

namespace EffectFramework
{
    internal interface IEffect
    {
        bool HasChanged();
        void Reset();
        void InvokeCallback();
    }
    
    public abstract class EffectComponent : MonoBehaviour
    {
        protected void UseEffect<TInstance, TDependency>(TInstance instance, string propertyToTrackName, Action callback)
            where TInstance : class
            where TDependency : struct
        {
            if (instance == null)
            {
                Debug.LogError($"Tried to get value from a null reference! Ensure components have dependencies before calling UseEffect");
                return;
            }
            
            var effect = new EffectDep<TInstance, TDependency>(instance, propertyToTrackName, callback);
            effects.Add(effect);
        }
        
        private List<IEffect> effects = new List<IEffect>();
        
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
        protected Prop<T> UseProp<T>(T e) where T : struct
        {
            return new Prop<T>(ScheduleRerender, e);
        }
        
        private bool isInit = false;

        [SerializeField] protected EffectComponent[] children;
        private EffectComponentManager _manager;    
        
        public void InitializeComponent(EffectComponentManager manager)
        {
            if (isInit)
            {
                return;
            }
            
            _manager = manager;
            _manager.RegisterComponent(this);
            
            OnMount();

            foreach (var child in children)
            {
                if (child == null) Debug.LogError($"There is am empty slot in children array on a GameObject named: {gameObject.name}");
            }
            
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
}
