using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class GraphicManager
{
    public List<Resolution> Resolutions { get; private set; } = new List<Resolution>();
    
    private ScriptableRendererFeature fullScreenRendererFeature;

    int curShader = 1;

    public void InitializeGraphics(ScriptableRendererData rendererData)
    {
        InitRendererFeature(rendererData);
        InitResolutions();

        GetShader();
    }

    private void InitRendererFeature(ScriptableRendererData rendererData)
    {
        foreach (var feature in rendererData.rendererFeatures)
        {
            if (feature is FullScreenPassRendererFeature)
            {
                fullScreenRendererFeature = feature;

                break;
            }
        }
    }

    private void InitResolutions()
    {
        Resolutions.AddRange(Screen.resolutions);
    }

    public void ChangeResolution(int index)
    {
        Screen.SetResolution(Resolutions[index].width, Resolutions[index].height, Screen.fullScreenMode);
    }

    public void ShaderOn()
    {
        fullScreenRendererFeature.SetActive(true);

        PlayerPrefs.SetInt("Shader", 1);
        PlayerPrefs.Save();
    }

    public void ShaderOff()
    {
        fullScreenRendererFeature.SetActive(false);

        PlayerPrefs.SetInt("Shader", 0);
        PlayerPrefs.Save();
    }

    public void ShaderOffWithoutSave()
    {
        fullScreenRendererFeature.SetActive(false);
    }

    private void GetShader()
    {
        int shaderState = PlayerPrefs.GetInt("Shader", curShader);

        if (shaderState == 1)
            fullScreenRendererFeature.SetActive(true);
        else
            fullScreenRendererFeature.SetActive(false);

        curShader = shaderState;
    }
}
