using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class TextureCreator : Image
{
    [SerializeField] private Texture2D[] textures;
    [SerializeField] private float speed;
    [SerializeField] private bool canSpin;
    [SerializeField] private float offset;
    
    private Texture2D m_fillingTexture;
    private Material m_slotMaterial;
    
    public override Material GetModifiedMaterial(Material baseMaterial)
    {
        Material cModifiedMat = base.GetModifiedMaterial(baseMaterial);

        if (!Application.isPlaying) return cModifiedMat;
        
        if (!canSpin)
        {
            offset %= 1;
        }
        else
        {
            offset += speed / 100;
            offset %= 1;
        }
        
        m_slotMaterial.SetFloat("_Offset", offset);

        return cModifiedMat;
    }

    protected override void Start()
    {
        base.Start();
        Texture2D slotTex = CreateFillingTexture();
        Sprite newSprite = Sprite.Create(slotTex, new Rect(0, 0, slotTex.width, slotTex.height), Vector2.zero);
        sprite = newSprite;
        offset = Random.Range(0, 1f);
        m_slotMaterial = new Material(material);
        m_slotMaterial.SetTexture("_MainTex", slotTex);
        m_slotMaterial.SetFloat("_Offset", offset);
        material = m_slotMaterial;
        SetNativeSize();
    }

    private Texture2D CreateFillingTexture()
    {
        int width = textures[0].width;
        int height = 0;
        int modHeight = textures[0].height;
            
        foreach (Texture2D t in textures)
        {
            height += t.height;
        }
            
        m_fillingTexture = new Texture2D(width, height);

        for (int i = 0; i < m_fillingTexture.width; i++)
        {
            for (int j = 0; j < m_fillingTexture.height; j++)
            {
                Color color = textures[j / modHeight].GetPixel(i, j % modHeight);
                m_fillingTexture.SetPixel(i, j, color);
            }
        }
            
        m_fillingTexture.Apply();

        return m_fillingTexture;
    }
        

    private void FixedUpdate()
    {
        material = GetModifiedMaterial(m_slotMaterial);
    }
}