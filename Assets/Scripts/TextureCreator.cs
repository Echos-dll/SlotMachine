using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class TextureCreator : Image
{
    [SerializeField] private Texture2D[] textures;
    [SerializeField] private float speed;
    [SerializeField] private bool canSpin;
    [SerializeField] private float offset;

    //private Image image;
    private Texture2D fillingTexture;
    private Material slotMaterial;

    // private void Awake()
    // {
    //     image = GetComponent<Image>();
    // }
        
    public override Material GetModifiedMaterial(Material baseMaterial)
    {
        Material cModifiedMat = base.GetModifiedMaterial(baseMaterial);

        if (!canSpin)
        {
            offset %= 1;
        }
        else
        {
            offset += speed / 100;
            offset %= 1;
        }
            
        // offset += speed / 100;
        // offset %= 1;
        slotMaterial.SetFloat("_Offset", offset);
            
        return cModifiedMat;
    }

    protected override void Start()
    {
        base.Start();
        Texture2D slotTex = CreateFillingTexture();
        Sprite newSprite = Sprite.Create(slotTex, new Rect(0, 0, slotTex.width, slotTex.height), Vector2.zero);
        sprite = newSprite;
        offset = Random.Range(0, 1f);
        slotMaterial = new Material(material);
        slotMaterial.SetTexture("_MainTex", slotTex);
        slotMaterial.SetFloat("_Offset", offset);
        material = slotMaterial;
        SetNativeSize();
        // material.SetTexture("_MainTex", slotTex);
        // material.SetFloat("_Offset", 0);
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
            
        fillingTexture = new Texture2D(width, height);

        for (int i = 0; i < fillingTexture.width; i++)
        {
            for (int j = 0; j < fillingTexture.height; j++)
            {
                Color color = textures[j / modHeight].GetPixel(i, j % modHeight);
                fillingTexture.SetPixel(i, j, color);
            }
        }
            
        fillingTexture.Apply();

        return fillingTexture;
    }
        

    private void FixedUpdate()
    {
        material = GetModifiedMaterial(slotMaterial);
    }
    //
    // private void SpinSlot()
    // {
    //     Debug.Log("TEST");
    //     if (!canSpin) return;
    //     offset += speed / 100;
    //     offset %= 1;
    //     material.SetFloat("_Offset", offset);
    //     
    // }
}