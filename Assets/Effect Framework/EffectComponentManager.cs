using System.Collections.Generic;
using UnityEngine;

namespace EffectFramework
{
    public class EffectComponentManager : MonoBehaviour
    {
        [SerializeField] private EffectComponent[] _componentsInScene;
        [SerializeField] private bool autoInitialize;
        private List<EffectComponent> components;
        private bool componentsInitialized = false;

        void Awake()
        {
            if (autoInitialize)
            {
                InitializeParentComponents();
            }
        }
    
        public void InitializeParentComponents()
        {
            components = new List<EffectComponent>();
        
            foreach (var component in _componentsInScene)
            {
                component.InitializeComponent(this);
            }

            componentsInitialized = true;
        }

        public void RegisterComponent(EffectComponent component)
        {
            components.Add(component);
        }

        void LateUpdate()
        {
            if (!componentsInitialized)
            {
                return;
            }
        
            components.ForEach(component => component.Run());
        }
    }
}
