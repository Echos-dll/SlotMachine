using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpinButton : MonoBehaviour
{
    [SerializeField] private ScriptableEvent _spinEvent;
    [SerializeField] private ScriptableEvent _stopEvent;
    [SerializeField] private ScriptableEvent _resultEvent;
    [SerializeField] private ScriptableEvent _skipEvent;
    [SerializeField] private TextMeshProUGUI _buttonText;

    private Button m_button;
    private bool m_canSpin;
    private bool m_isSpinning;

    private void Awake()
    {
        m_button = GetComponent<Button>();
    }

    private void ToggleSpin()
    {
        if (m_isSpinning)
        {
            Stop();
        }
        else
        {
            Spin();
        }
    }

    private void Stop()
    {
        _stopEvent.Invoke();
        m_isSpinning = false;
        ToggleButton(false);
        _buttonText.text = "SPIN";
    }

    private void Spin()
    {
        _spinEvent.Invoke();
        m_isSpinning = true;
        _buttonText.text = "STOP";
    }

    private void OnSkip()
    {
        m_isSpinning = false;
        _buttonText.text = "SPIN";
        ToggleButton(true);
    }

    private void OnResult()
    {
        m_isSpinning = false;
        ToggleButton(true);
    }

    private void ToggleButton(bool toggle)
    {
        m_button.interactable = toggle;
    }
    
    private void OnEnable()
    {
        m_button.onClick.AddListener(ToggleSpin);
        _skipEvent.AddListener(OnSkip);
        _resultEvent.AddListener(OnResult);
    }

    private void OnDisable()
    {
        m_button.onClick.RemoveListener(ToggleSpin);
        _skipEvent.RemoveListener(OnSkip);
        _resultEvent.RemoveListener(OnResult);
    }
}
