using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SlotMachine : MonoBehaviour
{
    [SerializeField] private List<SpinningSlot> _slots;
    [SerializeField] private List<SlotItem> _slotItems;
    [SerializeField] private List<Result> _results;
    [SerializeField] private ParticleSystem _coinParticleSystem;

    [Header("Settings")]
    [SerializeField] private float _spinSpeed;
    [SerializeField] private bool _slowStop;
    [SerializeField] private float _fastStopTime;
    [SerializeField] private float _normalStopTime;
    [SerializeField] private float _slowStopTime;
    
    [Header("Events")]
    [SerializeField] private ScriptableStateEvent _stateEvent;
    
    private RandomResultGenerator m_resultGenerator;
    
    private int m_stopCount;
    private int m_spinCount;
    private int m_currentResult;
    
    private void Awake()
    {
        Texture2D[] slotTextures = new Texture2D[_slotItems.Count];
        Texture2D[] blurTextures = new Texture2D[_slotItems.Count];

        for (int i = 0; i < slotTextures.Length; i++)
        {
            slotTextures[i] = _slotItems[i]._texture;
            blurTextures[i] = _slotItems[i]._textureBlur;
        }
        
        foreach (SpinningSlot slot in _slots)
            slot.SetTextures(slotTextures, blurTextures);

        m_resultGenerator = new RandomResultGenerator(_results);
    }

    private void Start()
    {
        m_resultGenerator.Load();
        SetSlotItemValues();
        
    }

    private void SetSlotItemValues()
    {
        for (int i = 0; i < _slotItems.Count; i++)
        {
            SlotItem item = _slotItems[i];
            item.SetValue(1f / _slotItems.Count * (i - (int)(_slotItems.Count / 2 )));
        }
    }

    private void StartSpinning()
    {
        float totalDelay = 0;
        foreach (SpinningSlot slot in _slots)
        {
            slot.StartWithDelay(totalDelay, _spinSpeed, OnSpinStarted);
            totalDelay += Random.value;
        }
    }

    private void OnSpinStarted()
    {
        m_spinCount++;
        if (m_spinCount != _slots.Count) return;
        
        _stateEvent.Invoke(State.Spinning);
        m_spinCount = 0;
    }
    
    private bool SlowMoCheck(int result)
    {
        return _results[result]._combination[0] == _results[result]._combination[1];
    }

    private void StopSpinning()
    {
        m_currentResult = m_resultGenerator.PickRandomResult();
        m_resultGenerator.Save();

        for (int i = 0; i < _slots.Count; i++)
        {
            SpinningSlot slot = _slots[i];

            if (i == _slots.Count - 1 && SlowMoCheck(m_currentResult))
            {
                slot.StopAtInTime(
                    _results[m_currentResult]._combination[i].GetValue()
                    ,_slowStop ? _slowStopTime : _normalStopTime
                    ,OnSpinStop);
            }
            else
            {
                slot.StopAtInTime(
                    _results[m_currentResult]._combination[i].GetValue()
                    ,(i + 1) * _fastStopTime
                    ,OnSpinStop);

            }
        }
    }
    
    private void OnSpinStop()
    {
        m_stopCount++;

        if (m_stopCount != _slots.Count) return;
        
        _stateEvent.Invoke(State.Result);
        m_stopCount = 0;
    }

    private void SkipSpinning()
    {
        m_currentResult = m_resultGenerator.PickRandomResult();
        m_resultGenerator.Save();
            
        for (int i = 0; i < _slots.Count; i++)
        {
            SpinningSlot slot = _slots[i];
            slot.CancelStartCoroutine();
            slot.StopAtInTime(_results[m_currentResult]._combination[i].GetValue(), 0, OnSpinStop);
        }
    }
    
    private void PlayParticle()
    {
        ParticleSystem.EmissionModule emissionModule = _coinParticleSystem.emission;
        emissionModule.rateOverTime = _results[m_currentResult]._particleCount;
        _coinParticleSystem.Play();
    }

    private void StateHandler(State state)
    {
        switch (state)
        {
            case State.Spin:
                StartSpinning();
                break;
            case State.Spinning:
                break;
            case State.Stop:
                StopSpinning();
                break;
            case State.Result:
                PlayParticle();
                break;
            case State.Skip:
                SkipSpinning();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

    private void OnEnable()
    {
        _stateEvent.AddListener(StateHandler);
    }

    private void OnDisable()
    {
        _stateEvent.RemoveListener(StateHandler);
    }
}

[Serializable]
public struct Result
{
    public SlotItem[] _combination;
    public float _chance;
    public int _reward;
    public int _particleCount;
}