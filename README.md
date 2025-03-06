# Effect – A React-Like Framework for Unity 🎮⚛️

**Effect** is a lightweight, component-based framework that brings **React-style reactivity** to **Unity's GameObject architecture**. It overrides Unity’s default lifecycle methods to ensure **perfectly synchronized state-driven updates** while preventing infinite loops.

With **`UseState`** and **`UseEffect`**, you can manage state dynamically—just like in React—but fully integrated into Unity's update cycle. Changes are tracked in **`Update`** and applied in **`LateUpdate`**, ensuring smooth and predictable UI updates.

## 🚀 Features
- **React-style component model** – Manage UI and game logic declaratively
- **`UseState` & `UseEffect`** – Track and react to changes efficiently
- **`Props`** - Pass props into sub components which will cause rerender
- **Eliminates infinite loops** – Updates are controlled by Unity's event cycle
- **Seamless integration** – Works with **DI frameworks, SOAP architecture, and Unity's `SerializeField`** pattern
- **Decouples UI from game logic** – Just provide the **state** and component will change
- **Props propagation** - Propagate props downwards to subcomponents

## 🎮 How It Works, Quick start 
Derive from UnityComponent
```csharp
public class FinishUI : UnityComponent
```

Register state and useEffect
```csharp
private State<bool> _on;

protected override void OnMount()
{
    _on = UseState(false);
    UseEffect<GameplayManager, bool>(_gameplayManager, nameof(_gameplayManager.MatchInProgress), () =>
    {
        _on.SetState(!_gameplayManager.MatchInProgress && _gameplayManager.NetworkUserType == NetworkUserType.Client);
    });
}
```
Reflect the change
```csharp
protected override void Rerender()
{
    gameObject.SetActive(_on.Value);
}
```

## Props
```csharp
private Prop<GameUIDataModel> props;
public GameUIDataModel Props { set => props.Value = value; }

protected override void OnMount()
{
    props = InitProp(new GameUIDataModel());
}
```

Once the prop is change the component will also rerender


