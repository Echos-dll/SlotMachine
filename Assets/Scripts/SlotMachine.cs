using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

public class SlotMachine : MonoBehaviour
{
    [SerializeField] private List<SpinningSlot> _slots;
    [SerializeField] private List<SlotItem> _slotItems;
    [SerializeField] private List<Result> _results;
    [SerializeField] private int _amountToPick;
    [SerializeField] private float _spinSpeed;
    [SerializeField] private bool _slowStop;
    
    [Header("Events")]
    [SerializeField] private ScriptableEvent _spinEvent;
    [SerializeField] private ScriptableEvent _stopEvent;
    [SerializeField] private ScriptableEvent _resultEvent;
    [SerializeField] private ScriptableEvent _skipEvent;
    
    private float[] m_pocket;
    private float[] m_remaining;
    private float m_leastChance;
    private float m_totalWeight;
    private int[] m_pickedAmounts;
    private int m_pickedCount;

    private void Awake()
    {
        m_remaining = new float[_results.Count];
        m_pocket = new float[_results.Count];
        m_pickedAmounts = new int[_results.Count];

        Texture2D[] textures = new Texture2D[_slotItems.Count];

        for (int i = 0; i < textures.Length; i++)
            textures[i] = _slotItems[i]._texture;
        
        foreach (SpinningSlot slot in _slots)
            slot.SetTextures(textures);
    }

    private void Start()
    {
        SetSlotItemValues();
        FindLeastChance();
        FillPocket();
        
        // for (int i = 0; i < _amountToPick; i++)
        // {
        //     PickRandomResult();
        //     m_pickedCount++;
        //     if (m_pickedCount % (100 / m_leastChance) == 0)
        //     {
        //         FillPocket();
        //     }
        // }

        // for (int i = 0; i < m_pickedAmounts.Length; i++)
        // {
        //     Debug.Log($"Index: {i} Picked " + m_pickedAmounts[i] + " times");
        // }
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
        
        float pick = UnityEngine.Random.value * m_totalWeight;
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
        for (int i = 0; i < _slots.Count; i++)
        {
            SpinningSlot slot = _slots[i];
            slot.StartWithDelay(totalDelay, _spinSpeed);
            totalDelay += Random.value;
        }
    }

    private void StopSpinning()
    {
        int result = PickRandomResult();
        
        for (int i = 0; i < _slots.Count; i++)
        {
            SpinningSlot slot = _slots[i];
            
            if (i == _slots.Count - 1 && SlowMoCheck(result))
            {
                slot.StopAtInTime(_results[result]._combination[i].GetValue(), _slowStop ? 2.5f : 1f,
                    _resultEvent.Invoke);
            }
            else
            {
                slot.StopAtInTime(_results[result]._combination[i].GetValue(),  (i + 1) * .04f, _resultEvent.Invoke);

            }
        }
    }

    private bool SlowMoCheck(int result)
    {
        return _results[result]._combination[0] == _results[result]._combination[1];
    }

    private void SkipSpinning()
    {
        int result = PickRandomResult();
        
        for (int i = 0; i < _slots.Count; i++)
        {
            SpinningSlot slot = _slots[i];
            slot.CancelCoroutine();
            slot.StopAtInTime(_results[result]._combination[i].GetValue(), 0, _resultEvent.Invoke);
        }
    }

    private void InfoLogger()
    {
        foreach (Result result in _results)
        {
            StringBuilder strBuilder = new StringBuilder();

            foreach (SlotItem item in result._combination)
            {
                strBuilder.Append($"{item.name} ");
            }

            strBuilder.Append($" Chance: {result._chance} Reward: {result._reward}");

            Debug.Log(strBuilder.ToString());
        }
    }

    private void OnEnable()
    {
        _spinEvent.AddListener(StartSpinning);
        _stopEvent.AddListener(StopSpinning);
        _skipEvent.AddListener(SkipSpinning);
    }

    private void OnDisable()
    {
        _spinEvent.RemoveListener(StartSpinning);
        _stopEvent.RemoveListener(StopSpinning);
        _skipEvent.RemoveListener(SkipSpinning);
    }
}

[Serializable]
public struct Result
{
    public SlotItem[] _combination;
    public float _chance;
    public int _reward;
}