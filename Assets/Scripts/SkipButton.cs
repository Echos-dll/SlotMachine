using System;
using UnityEngine;
using UnityEngine.UI;

public class SkipButton : MonoBehaviour
{
    [SerializeField] private ScriptableEvent _skipEvent;

    private Button m_button;

    private void Awake()
    {
        m_button = GetComponent<Button>();
    }

    private void Skip()
    {
        _skipEvent.Invoke();
    }
        
    private void OnEnable()
    {
        m_button.onClick.AddListener(Skip);
    }

    private void OnDisable()
    {
        m_button.onClick.RemoveListener(Skip);
    }
}