using System.Collections.Generic;
using UnityEngine;

public class RandomResultGenerator
{
    private readonly List<Result> m_results;
    
    private float[] m_pocket;
    private float[] m_remaining;
    private int[] m_pickedAmounts;

    private float m_leastChance;
    private float m_totalWeight;
    private int m_pickedCount;
    private bool m_loaded;

        
    public RandomResultGenerator(List<Result> results)
    {
        m_results = results;
        FindLeastChance();
        Load();
        if (!m_loaded) FillPocket();
    }
        
    private void FillPocket()
    {
        m_remaining = m_pocket;

        m_pocket = new float[m_results.Count];
        
        for (int i = 0; i < m_pocket.Length; i++)
        {
            m_pocket[i] = m_results[i]._chance / m_leastChance + m_remaining[i];
        }
    }
        
    public int PickRandomResult()
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

        Save();
        return chosenIndex;
    }
        
    private void FindLeastChance()
    {
        float least = 100;
        
        foreach (Result result in m_results)
        {
            if (result._chance < least)
            {
                least = result._chance;
            }
        }
        
        m_leastChance = least;
    }
    
    public void DebugRandomResult(int amountToPick)
    {
        m_pocket = new float[m_results.Count];
        m_pickedAmounts = new int[m_results.Count];
            
        m_pickedCount = 0;

        FillPocket();
            
        for (int i = 0; i < amountToPick; i++)
        {
            PickRandomResult();
        }

        for (int i = 0; i < m_pickedAmounts.Length; i++)
        {
            Debug.Log($"Index: {i} Picked " + m_pickedAmounts[i] + " times");
        }
    }
    
    public void DebugLoad()
    {
        for (int i = 0; i < m_pickedAmounts.Length; i++)
        {
            Debug.Log("Index: " + i + " Picked " + m_pickedAmounts[i] + " times");
        }
    }
    
    public void Save()
    {
        SaveData save = new SaveData
        {
            _pocketData = m_pocket,
            _pickedCountData = m_pickedCount,
            _pickedAmountsData = m_pickedAmounts
        };
        
        SaveSystem.Save(save);
    }

    private void Load()
    {
        if (SaveSystem.TryLoad(out SaveData load))
        {
            m_pocket = load._pocketData;
            m_pickedCount = load._pickedCountData;
            m_pickedAmounts = load._pickedAmountsData;
            m_loaded = true;
            
            return;
        }
        
        m_pocket = new float[m_results.Count];
        m_pickedAmounts = new int[m_results.Count];
    }

    
}