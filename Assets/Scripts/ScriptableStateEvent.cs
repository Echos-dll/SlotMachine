using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Scriptable State Event", menuName = "Create Scriptable State Event")]
public class ScriptableStateEvent : ScriptableObject
{
    private Action<State> m_event;
    
    public void AddListener(Action<State> action)
    {
        m_event += action;
    }
    
    public void RemoveListener(Action<State> action)
    {
        m_event -= action;
    }
    
    public void Invoke(State state)
    {
        m_event?.Invoke(state);
    }
}

public enum State
{
    Spin,
    Spinning,
    Stop,
    Result,
    Skip
}