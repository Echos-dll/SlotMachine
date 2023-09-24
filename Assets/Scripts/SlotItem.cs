using UnityEngine;

[CreateAssetMenu(fileName = "New Slot Item", menuName = "Create Slot Item", order = 0)]
public class SlotItem : ScriptableObject
{
    public Sprite _sprite;
    public Sprite _spriteBlur;
    private int m_value;

    public void SetValue(int val)
    {
        m_value = val;
    }

    public int GetValue()
    {
        return m_value;
    }
}