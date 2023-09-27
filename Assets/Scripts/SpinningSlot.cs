using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SpinningSlot : Image
{
    private Texture2D[] m_slotTextures;
    private Texture2D[] m_blurTextures;
    private Texture2D m_fillingTexture;
    private Material m_slotMaterial;
    private Coroutine m_startRoutine;
    private Tween m_stopTween;
    
    private float m_maxSpeed;
    private float m_currentSpeed;        
    private bool m_canSpin;       
    private float m_offset;
    private float m_transition;
    private bool m_stopping;
    
    protected override void Start()
    {
        base.Start();
        if (!Application.isPlaying) return;

        Texture2D slotTex = CreateSlotTexture();
        Texture2D blurTex = CreateBlurTexture();
        Sprite newSprite = Sprite.Create(slotTex, new Rect(0, 0, slotTex.width, slotTex.height), Vector2.zero);
        sprite = newSprite;
        m_offset = 0;
        m_transition = 1;
        m_slotMaterial = new Material(material);
        m_slotMaterial.SetTexture("_MainTex", slotTex);
        m_slotMaterial.SetTexture("_BlurTex", blurTex);
        m_slotMaterial.SetFloat("_Offset", m_offset);
        m_slotMaterial.SetFloat("_Transition", m_transition);
        material = m_slotMaterial;
        SetNativeSize();
        m_canSpin = false;
    }
    
    public override Material GetModifiedMaterial(Material baseMaterial)
    {
        Material cModifiedMat = base.GetModifiedMaterial(baseMaterial);

        if (!Application.isPlaying) return cModifiedMat;
        
        if (!m_canSpin && !m_stopping)
        {
            m_offset %= 1;
        }
        else
        {
            m_offset += m_currentSpeed / 100;
            m_offset %= 1;
        }
        
        m_slotMaterial.SetFloat("_Transition", m_transition);
        m_slotMaterial.SetFloat("_Offset", m_offset);

        return cModifiedMat;
    }

    private Texture2D CreateSlotTexture()
    {
        int width = m_slotTextures[0].width;
        int height = 0;
        int modHeight = m_slotTextures[0].height;
            
        foreach (Texture2D t in m_slotTextures)
        {
            height += t.height;
        }
            
        m_fillingTexture = new Texture2D(width, height);

        for (int i = 0; i < m_fillingTexture.width; i++)
        {
            for (int j = 0; j < m_fillingTexture.height; j++)
            {
                Color pixel = m_slotTextures[j / modHeight].GetPixel(i, j % modHeight);
                m_fillingTexture.SetPixel(i, j, pixel);
            }
        }
            
        m_fillingTexture.Apply();

        return m_fillingTexture;
    }
    
    private Texture2D CreateBlurTexture()
    {
        int width = m_blurTextures[0].width;
        int height = 0;
        int modHeight = m_blurTextures[0].height;
            
        foreach (Texture2D t in m_blurTextures)
        {
            height += t.height;
        }
            
        m_fillingTexture = new Texture2D(width, height);

        for (int i = 0; i < m_fillingTexture.width; i++)
        {
            for (int j = 0; j < m_fillingTexture.height; j++)
            {
                Color pixel = m_blurTextures[j / modHeight].GetPixel(i, j % modHeight);
                m_fillingTexture.SetPixel(i, j, pixel);
            }
        }
            
        m_fillingTexture.Apply();

        return m_fillingTexture;
    }
    
    public void StopAtInTime(float offset, float duration, Action callbackAction)
    {
        if (m_stopTween != null && m_stopTween.IsPlaying())
        {
            m_stopTween.Kill(true);
            return;
        }
        
        m_stopping = true;
        
        m_transition = 1;

        float from = m_offset;
        float to = offset + 1;
        
        void OnVirtualUpdate(float value)
        {
            if (from > to) to++;
            
            m_offset = Mathf.Lerp(from, to, value) + 1;
        }
        
        m_stopTween = DOVirtual.Float(0,1,duration, OnVirtualUpdate).OnComplete(() =>
        {
            m_stopTween = null;
            m_canSpin = false;
            m_stopping = false;
            m_currentSpeed = 0;
            m_offset = offset;
            callbackAction?.Invoke();
        });
    }

    public void StartWithDelay(float delay, float spinSpeed)
    {
        m_startRoutine = StartCoroutine(StartSpin(delay, spinSpeed));
    }
    
    public void CancelCoroutine()
    {
        if (m_startRoutine != null)
            StopCoroutine(m_startRoutine);
    }

    private IEnumerator StartSpin(float delay, float spinSpeed)
    {
        yield return new WaitForSeconds(delay);
        m_canSpin = true;
        m_maxSpeed = spinSpeed;
        DOVirtual.Float(m_currentSpeed, m_maxSpeed, 1f, (x) =>
        {
            m_currentSpeed = x;
            m_transition = 1 - Mathf.Lerp(0, 1, Mathf.InverseLerp(0, m_maxSpeed, m_currentSpeed));
        });
    }
    
    public void SetTextures(Texture2D[] slot, Texture2D[] blur)
    {
        m_slotTextures = slot;
        m_blurTextures = blur;
    }
    
    private void FixedUpdate()
    {
        material = GetModifiedMaterial(m_slotMaterial);
    }
}