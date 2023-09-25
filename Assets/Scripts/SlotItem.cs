using UnityEngine;

[CreateAssetMenu(fileName = "New Slot Item", menuName = "Create Slot Item", order = 0)]
public class SlotItem : ScriptableObject
{
    public Texture2D _texture;
    public Texture2D _textureBlur;
    private float m_value;

    public void SetValue(float val)
    {
        m_value = val;
    }

    public float GetValue()
    {
        return m_value;
    }
}