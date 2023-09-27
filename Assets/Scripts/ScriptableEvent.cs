using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Scriptable Event", menuName = "Create Scriptable Event")]
public class ScriptableEvent : ScriptableObject
{
    private Action m_event;
    
    public void AddListener(Action action)
    {
        m_event += action;
    }
    
    public void RemoveListener(Action action)
    {
        m_event -= action;
    }
    
    public void Invoke()
    {
        m_event?.Invoke();
    }
}