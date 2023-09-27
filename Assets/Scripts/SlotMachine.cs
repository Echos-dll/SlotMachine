using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;

public class SlotMachine : MonoBehaviour
{
    [SerializeField] private List<SpinningSlot> _slots;
    [SerializeField] private List<SlotItem> _slotItems;
    [SerializeField] private List<Result> _results;
    [SerializeField] private float _spinSpeed;
    [SerializeField] private bool _slowStop;
    [SerializeField] private ParticleSystem _coinParticleSystem;
    
    [Header("Events")]
    [SerializeField] private ScriptableStateEvent _stateEvent;
    
    [Header("DEBUG MODE")]
    [SerializeField] private bool _debugMode;
    [SerializeField] private int _amountToPick;
    
    private float[] m_pocket;
    private float[] m_remaining;
    private float m_leastChance;
    private float m_totalWeight;
    private int[] m_pickedAmounts;
    private int m_stopCount;
    private int m_spinCount;
    private int m_pickedCount;
    private int m_currentResult;
    private bool m_loaded;

    private void Awake()
    {
        Load();
        
        Texture2D[] slotTextures = new Texture2D[_slotItems.Count];
        Texture2D[] blurTextures = new Texture2D[_slotItems.Count];

        for (int i = 0; i < slotTextures.Length; i++)
        {
            slotTextures[i] = _slotItems[i]._texture;
            blurTextures[i] = _slotItems[i]._textureBlur;
        }
        
        foreach (SpinningSlot slot in _slots)
            slot.SetTextures(slotTextures, blurTextures);
    }

    private void Start()
    {
        SetSlotItemValues();
        FindLeastChance();
        if (!m_loaded) FillPocket();

        if (_debugMode)
        {
            m_pocket = new float[_results.Count];
            m_pickedAmounts = new int[_results.Count];
            
            m_pickedCount = 0;

            FillPocket();
            
            for (int i = 0; i < _amountToPick; i++)
            {
                PickRandomResult();
            }

            for (int i = 0; i < m_pickedAmounts.Length; i++)
            {
                Debug.Log($"Index: {i} Picked " + m_pickedAmounts[i] + " times");
            }
        }
    }

    private void SetSlotItemValues()
    {
        for (int i = 0; i < _slotItems.Count; i++)
        {
            SlotItem item = _slotItems[i];
            item.SetValue(1f / _slotItems.Count * (i - (int)(_slotItems.Count / 2 )));
        }
    }

    private void FillPocket()
    {
        m_remaining = m_pocket;

        m_pocket = new float[_results.Count];
        
        for (int i = 0; i < m_pocket.Length; i++)
        {
            m_pocket[i] = _results[i]._chance / m_leastChance + m_remaining[i];
        }
    }

    private int PickRandomResult()
    {
        m_totalWeight = 0;
        
        for (int i = 0; i < m_pocket.Length; i++)
        {
            m_totalWeight += m_pocket[i];
        }
        
        float pick = Random.value * m_totalWeight;
        int chosenIndex = 0;
        float cumulativeWeight = m_pocket[0];
        
        while (pick > cumulativeWeight && chosenIndex < m_pocket.Length - 1)
        {
            chosenIndex++;
            cumulativeWeight += m_pocket[chosenIndex];
        }
        
        m_pocket[chosenIndex]--;
        m_pickedAmounts[chosenIndex]++;
        m_pickedCount++;
        
        if (m_pickedCount % (100 / m_leastChance) == 0)
            FillPocket();

        return chosenIndex;
    }

    private void FindLeastChance()
    {
        float least = 100;
        
        foreach (Result result in _results)
        {
            if (result._chance < least)
            {
                least = result._chance;
            }
        }
        
        m_leastChance = least;
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
        m_currentResult = PickRandomResult();
        
        for (int i = 0; i < _slots.Count; i++)
        {
            SpinningSlot slot = _slots[i];

            if (i == _slots.Count - 1 && SlowMoCheck(m_currentResult))
            {
                slot.StopAtInTime(_results[m_currentResult]._combination[i].GetValue(), _slowStop ? 2.5f : 1f,
                    OnSpinStop);
            }
            else
            {
                slot.StopAtInTime(_results[m_currentResult]._combination[i].GetValue(), (i + 1) * .04f,
                    OnSpinStop);

            }
        }
    }
    
    private void OnSpinStop()
    {
        m_stopCount++;

        if (m_stopCount != _slots.Count) return;
        
        //_resultEvent.Invoke();
        _stateEvent.Invoke(State.Result);
        m_stopCount = 0;
    }

    private void SkipSpinning()
    {
        m_currentResult = PickRandomResult();
        
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

    private void Save()
    {
        SaveData save = new SaveData
        {
            _pocketData = m_pocket,
            _pickedCountData = m_pickedCount,
            _pickedAmountsData = m_pickedAmounts
        };

        string data = JsonUtility.ToJson(save);
        PlayerPrefs.SetString("SaveData", data);
    }

    private void Load()
    {
        m_pocket = new float[_results.Count];
        m_pickedAmounts = new int[_results.Count];

        if (!PlayerPrefs.HasKey("SaveData")) return;
        
        SaveData load = JsonUtility.FromJson<SaveData>(PlayerPrefs.GetString("SaveData"));
        m_pocket = load._pocketData;
        m_pickedCount = load._pickedCountData;
        m_pickedAmounts = load._pickedAmountsData;
        m_loaded = true;
    }

    private void OnEnable()
    {
        _stateEvent.AddListener(StateHandler);
    }

    private void OnDisable()
    {
        _stateEvent.RemoveListener(StateHandler);
    }

    private void OnDestroy()
    {
        Save();
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

[Serializable]
public struct SaveData
{
    [JsonProperty("pcd")] public int _pickedCountData;
    [JsonProperty("pd")] public float[] _pocketData;
    [JsonProperty("pa")] public int[] _pickedAmountsData;
}