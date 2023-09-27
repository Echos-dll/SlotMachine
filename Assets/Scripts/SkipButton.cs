using UnityEngine;
using UnityEngine.UI;

public class SkipButton : MonoBehaviour
{
    [SerializeField] private ScriptableStateEvent _stateEvent;

    private Button m_button;

    private void Awake()
    {
        m_button = GetComponent<Button>();
    }

    private void Skip()
    {
        _stateEvent.Invoke(State.Skip);
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