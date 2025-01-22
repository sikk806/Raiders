using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class TextureQuality : MonoBehaviour
{
    // [SerializeField] private TMP_Text qualityText;
    [SerializeField] private TMP_Dropdown qualiftyDropdown;
    // High = Ultra  MIddle =middle  Low = veryLow
    // Ultra = 0 , high  = 1, middle = 2 , low =3  , verylow =4;
    private List<string> _levelDescriptions = new() { "High", "Middle", "Low" };
    [SerializeField]
    List<RenderPipelineAsset> RenderPipelineAssets;
    void Start()
    {
        qualiftyDropdown.ClearOptions();
        qualiftyDropdown.AddOptions(_levelDescriptions);
        
        // RefreshTextureQuality(QualitySettings.globalTextureMipmapLimit);
        
        
        // RefreshQualityText(QualitySettings.globalTextureMipmapLimit);
        
        qualiftyDropdown.onValueChanged.AddListener(ChangeOption);
    }

    /// <summary>
    /// 텍스처의 Mipmap의 해상도 퀄리티를 변경합니다.
    /// </summary>
    /// <param name="qualityLevel">변경할 해상도</param>
    private void ChangeOption(int qualityLevel)
    {
        // RefreshQualityText(qualityLevel);
        // RefreshTextureQuality(qualityLevel);
        SetPipeline(qualityLevel);
    }
    
    /// <summary>
    /// 텍스처의 Mipmap의 해상도 퀄리티를 변경합니다.
    /// </summary>
    /// <param name="qualityLevel">변경할 해상도</param>
    // private void RefreshTextureQuality(int qualityLevel)
    // {
    //     qualiftyDropdown.value = qualityLevel;
    //     //수식만 처리하기
    //     QualitySettings.globalTextureMipmapLimit = qualityLevel;
    // }
    
    public void SetPipeline(int value)
    {
        QualitySettings.SetQualityLevel(value);
        QualitySettings.renderPipeline = RenderPipelineAssets[value];
    }
    
    /// <summary>
    /// 텍스처의 Mipmap의 해상도 퀄리티에 맞춰 글을 변경합니다.
    /// </summary>
    // /// <param name="index">변경할 해상도</param>
    // private void RefreshQualityText(int index)
    // {
    //     qualityText.text = $"{_levelDescriptions[index]}";
    // }
}
