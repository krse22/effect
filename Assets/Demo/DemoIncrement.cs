using EffectFramework;
using EffectFramework.Rerender;
using TMPro;
using UnityEngine;

public class DemoIncrement : EffectComponent
{

    [SerializeField] private TMP_Text text;
    
    private State<int> _counter;

    protected override void OnMount()
    {
        _counter = UseState<int>(0);
    }

    protected override void Rerender()
    {
        text.text = _counter.Value.ToString();
    }
    
    public void Increment()
    {
        _counter.SetState(_counter.Value + 1);
    }

}
