using System.Collections.Generic;
using UnityEngine;

public class UnityComponentManager : MonoBehaviour
{
    
    [SerializeField] private UnityComponent[] _componentsInScene;

    private List<UnityComponent> components;
    private bool componentsInitialized = false;
    
    public void InitializeParentComponents()
    {
        components = new List<UnityComponent>();
        
        foreach (var component in _componentsInScene)
        {
            component.InitializeComponent(this);
        }

        componentsInitialized = true;
    }

    public void RegisterComponent(UnityComponent component)
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
