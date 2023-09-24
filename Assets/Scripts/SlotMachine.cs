using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class SlotMachine : MonoBehaviour
{
    [SerializeField] private List<SlotItem> _slotItems;
    [SerializeField] private List<Result> _results;
    [SerializeField] private int _amountToPick;
    
    
    private float[] m_pocket;
    private float m_leastChance;
    private float m_totalWeight;
    private int[] m_pickedAmounts;
    private int m_pickedCount;
    

    private void Start()
    {
        m_pickedAmounts = new int[_results.Count];
        FindLeastChance();
        FillPocket();
        
        for (int i = 0; i < _amountToPick; i++)
        {
            PickRandomResult();
            m_pickedCount++;
            if (m_pickedCount % (100 / m_leastChance) == 0)
            {
                FillPocket();
            }
        }

        for (int i = 0; i < m_pickedAmounts.Length; i++)
        {
            Debug.Log($"Index: {i} Picked " + m_pickedAmounts[i] + " times");
        }
    }

    private void FillPocket()
    {
        float[] remaining = new float[_results.Count];
        
        if (m_pocket.Length != 0)
            remaining = m_pocket;

        m_pocket = new float[_results.Count];
        
        for (int i = 0; i < m_pocket.Length; i++)
        {
            m_pocket[i] = _results[i]._chance / m_leastChance + remaining[i];
        }
    }

    private void PickRandomResult()
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
}

[Serializable]
public struct Result
{
    public SlotItem[] _combination;
    public float _chance;
    public int _reward;
}