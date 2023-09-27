using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpinButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _buttonText;
    [SerializeField] private ScriptableStateEvent _stateEvent;

    private Button m_button;
    private bool m_canSpin;
    private bool m_isSpinning;

    private void Awake()
    {
        m_button = GetComponent<Button>();
    }

    private void StateHandler(State state)
    {
        switch (state)
        {
            case State.Spin:
                ToggleButton(false);
                break;
            case State.Spinning:
                ToggleButton(true);
                break;
            case State.Stop:
                break;
            case State.Result:
                OnResult();
                break;
            case State.Skip:
                OnSkip();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
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
        _stateEvent.Invoke(State.Stop);
        m_isSpinning = false;
        ToggleButton(false);
        _buttonText.text = "SPIN";
    }

    private void Spin()
    {
        _stateEvent.Invoke(State.Spin);
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
        _stateEvent.AddListener(StateHandler);
    }

    private void OnDisable()
    {
        m_button.onClick.RemoveListener(ToggleSpin);
        _stateEvent.RemoveListener(StateHandler);
    }
}
